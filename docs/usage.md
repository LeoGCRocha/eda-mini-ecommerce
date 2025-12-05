# Usage Guide

## Table of Contents
1. [Getting Started](#getting-started)
2. [Common Workflows](#common-workflows)
3. [Event-Driven Scenarios](#event-driven-scenarios)
4. [Monitoring & Observability](#monitoring--observability)
5. [Testing & Debugging](#testing--debugging)
6. [Production Considerations](#production-considerations)

---

## Getting Started

### Quick Start

1. **Start the Application**
   ```bash
   cd eda-mini-ecommerce
   dotnet run --project src/EdaMicroEcommerce.AppHost/EdaMicroEcommerce.AppHost.csproj
   ```

2. **Access the Dashboard**
   - Open browser to `http://localhost:15000` (Aspire Dashboard)
   - View all running services and their URLs

3. **Open Swagger UI**
   - Find the API URL in Aspire Dashboard
   - Navigate to `<api-url>/swagger`

4. **Check Kafka**
   - Open KafkaUI at `http://localhost:9100`
   - View topics and messages

---

## Common Workflows

### Workflow 1: Create and Process an Order

This is the primary workflow demonstrating the complete event-driven order processing saga.

#### Step 1: Create a Product

**Request**:
```bash
curl -X POST http://localhost:<port>/api/v1/product \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Gaming Laptop",
    "description": "High-performance gaming laptop with RTX 4080",
    "price": 1899.99,
    "availableQuantity": 25,
    "reorderQuantity": 5
  }'
```

**Response**: `200 OK`

**What Happens**:
1. Product created in `catalog.products` table
2. Inventory item created in `catalog.inventory_items` table
3. Product is now available for ordering

#### Step 2: Create an Order

**Request**:
```bash
curl -X POST http://localhost:<port>/api/v1/order \
  -H "Content-Type: application/json" \
  -d '{
    "customerId": "123e4567-e89b-12d3-a456-426614174000",
    "orderItemDtos": [
      {
        "productId": "<product-id-from-step-1>",
        "quantity": 2
      }
    ]
  }'
```

**Response**: `204 No Content`

**What Happens**:
1. Order created with status `Pending`
2. `OrderCreatedEvent` saved to `orders.outbox_integration_events`
3. Order creation completes

#### Step 3: Background Event Processing

**Automatic Events Flow** (no user action required):

```
1. Outbox Worker (every 5s)
   └─> Publishes order-created event to Kafka

2. Saga Orchestrator consumes order-created
   ├─> Creates saga instance in database
   └─> Publishes product-reservation event

3. Inventory Worker consumes product-reservation
   ├─> Checks available stock
   ├─> Reserves inventory if available
   └─> Publishes product-reserved event (success/failure)

4. Saga Orchestrator consumes product-reserved
   ├─> If success: Publishes payment-pending event
   └─> If failure: Cancels order (compensation)

5. Payment Worker consumes payment-pending
   ├─> Processes payment (90% success simulation)
   └─> Publishes payment-processed event

6. Saga Orchestrator consumes payment-processed
   ├─> If success: Marks order as Confirmed
   └─> If failure: Releases inventory, cancels order
```

#### Step 4: Monitor the Flow

**Option A: KafkaUI**
1. Open `http://localhost:9100`
2. Navigate to Topics
3. View messages in each topic:
   - `order-created`
   - `product-reservation`
   - `product-reserved`
   - `payment-pending`
   - `payment-processed`

**Option B: Aspire Dashboard**
1. Open `http://localhost:15000`
2. View Traces tab
3. See distributed trace for the entire order flow

**Option C: Database**
```sql
-- Check order status
SELECT order_id, status, created_at, updated_at 
FROM orders.orders 
ORDER BY created_at DESC 
LIMIT 1;

-- Check saga state
SELECT * FROM orders.saga_events 
WHERE order_id = '<your-order-id>';

-- Check inventory
SELECT product_id, quantity_available, reserved_quantity 
FROM catalog.inventory_items;
```

#### Step 5: Verify Completion

**Expected Final State** (after 10-30 seconds):
- Order status: `Confirmed`
- Inventory: `quantity_available` reduced by 2
- Payment: Record in `billing.payments` with status `Processed`
- Saga: Completed successfully

---

### Workflow 2: Product Deactivation

Demonstrates event propagation across modules.

#### Step 1: Deactivate Product

**Request**:
```bash
curl -X POST http://localhost:<port>/api/v1/product/<product-id>/deactivate
```

**Response**: `200 OK`

#### Step 2: Observe Events

**KafkaUI** → Topic: `product-deactivated`
```json
{
  "productId": {
    "value": "550e8400-e29b-41d4-a716-446655440000"
  }
}
```

#### Step 3: Verify Side Effects

**Database Check**:
```sql
-- Product marked inactive
SELECT product_id, name, is_active 
FROM catalog.products 
WHERE product_id = '<product-id>';

-- Inventory worker processed event
SELECT * FROM catalog.processed_events 
WHERE event_type = 'product-deactivated';
```

**Expected Behavior**:
- Product cannot be included in new orders
- Existing orders with this product are unaffected

---

### Workflow 3: Handling Failures

Demonstrates saga compensation when steps fail.

#### Scenario A: Insufficient Inventory

**Setup**:
1. Create product with `availableQuantity: 5`
2. Create order with `quantity: 10`

**Expected Flow**:
```
1. Order created → Pending
2. product-reservation published
3. Inventory Worker: Insufficient stock
4. product-reserved published with failure status
5. Saga Orchestrator: Compensation
6. Order cancelled
```

**Verification**:
```sql
SELECT order_id, status 
FROM orders.orders 
WHERE order_id = '<order-id>';
-- Expected: status = 'Cancelled'
```

#### Scenario B: Payment Failure

**Setup**:
1. Create valid order
2. Payment Worker randomly fails (10% chance)

**Expected Flow**:
```
1-3. Normal flow (order, inventory reservation)
4. payment-processed published with 'Failed' status
5. Saga Orchestrator: Compensation
   ├─> Releases inventory
   └─> Cancels order
```

**Verification**:
```sql
-- Order cancelled
SELECT status FROM orders.orders WHERE order_id = '<order-id>';

-- Inventory released
SELECT reserved_quantity FROM catalog.inventory_items WHERE product_id = '<product-id>';
-- Expected: reserved_quantity unchanged or reduced
```

---

## Event-Driven Scenarios

### Scenario 1: Event Replay (Disaster Recovery)

**Purpose**: Rebuild system state from event stream

**Steps**:

1. **Backup Current State**
   ```bash
   pg_dump -h localhost -p 5450 -U postgres EdaMicroDb > backup.sql
   ```

2. **Reset Consumer Offsets**
   ```bash
   # Using Kafka CLI
   docker exec -it <kafka-container> kafka-consumer-groups \
     --bootstrap-server localhost:9092 \
     --group order-saga-orchestrator \
     --topic order-created \
     --reset-offsets --to-earliest --execute
   ```

   **Alternative**: KafkaUI
   - Navigate to Consumer Groups
   - Select group (e.g., `order-saga-orchestrator`)
   - Reset offsets to beginning

3. **Clear Saga State** (optional)
   ```sql
   TRUNCATE TABLE orders.saga_events;
   ```

4. **Restart Services**
   ```bash
   # Restart saga orchestrator
   dotnet run --project src/Orders.SagaOrchestrator
   ```

5. **Monitor Replay**
   - Watch Aspire Dashboard for traces
   - Verify saga events being recreated

**Use Cases**:
- Recover from data corruption
- Test event handling logic
- Debug production issues in dev environment

---

### Scenario 2: Adding a New Event Subscriber

**Example**: Add a notification module that sends emails when orders are confirmed

**Implementation Steps**:

1. **Create New Worker Module**
   ```bash
   dotnet new worker -n Notifications.EmailWorker
   ```

2. **Configure Kafka Consumer**
   ```csharp
   services.AddKafka(kafka => kafka
       .AddCluster(cluster => cluster
           .AddConsumer(consumer => consumer
               .Topic("order-created")
               .WithGroupId("email-notification-worker")
               .WithAutoOffsetReset(AutoOffsetReset.Earliest)
               .AddTypedHandlers(h => h
                   .AddHandler<OrderCreatedEmailHandler>()
               )
           )
       )
   );
   ```

3. **Implement Handler**
   ```csharp
   public class OrderCreatedEmailHandler : IMessageHandler<OrderCreatedEvent>
   {
       public async Task Handle(IMessageContext context, OrderCreatedEvent message)
       {
           // Send email
           await _emailService.SendOrderConfirmationEmail(message.OrderId);
       }
   }
   ```

4. **Register in AppHost**
   ```csharp
   builder.AddProject<Projects.Notifications_EmailWorker>("email-worker")
       .WithReference(kafka);
   ```

**Benefits of EDA**:
- Existing modules unchanged
- No deployment dependencies
- Can replay past events to send missed notifications

---

### Scenario 3: Implementing Idempotency

**Challenge**: Consumer processes same message twice due to retry

**Solution**: Track processed event IDs

**Implementation**:

```csharp
public class IdempotentOrderCreatedHandler : IMessageHandler<OrderCreatedEvent>
{
    private readonly ApplicationDbContext _dbContext;
    
    public async Task Handle(IMessageContext context, OrderCreatedEvent message)
    {
        // Check if already processed
        var eventId = $"order-created-{message.OrderId.Value}";
        
        var alreadyProcessed = await _dbContext.ProcessedEvents
            .AnyAsync(e => e.EventId == eventId);
        
        if (alreadyProcessed)
        {
            _logger.LogInformation("Event {EventId} already processed, skipping", eventId);
            return; // Idempotent exit
        }
        
        // Process event
        var saga = new SagaEvent { OrderId = message.OrderId, Status = "Started" };
        _dbContext.SagaEvents.Add(saga);
        
        // Record processing
        _dbContext.ProcessedEvents.Add(new ProcessedEvent 
        { 
            EventId = eventId,
            ProcessedAt = DateTime.UtcNow 
        });
        
        await _dbContext.SaveChangesAsync();
    }
}
```

**Verification**:
```sql
SELECT * FROM processed_events ORDER BY processed_at DESC;
```

---

## Monitoring & Observability

### Distributed Tracing

**Access Traces**:
1. Open Aspire Dashboard: `http://localhost:15000`
2. Navigate to "Traces" tab
3. Filter by service or operation

**Example Trace View**:
```
Trace ID: 3fa85f64-5717-4562-b3fc-2c963f66afa6
Duration: 245ms

├─ [API] POST /api/v1/order (50ms)
│  └─ [DB] Insert Order (15ms)
│  └─ [DB] Insert OutboxEvent (5ms)
│
├─ [OutboxWorker] Publish order-created (10ms)
│  └─ [Kafka] Send message (8ms)
│
├─ [SagaOrchestrator] Handle order-created (120ms)
│  ├─ [DB] Create SagaEvent (10ms)
│  └─ [Kafka] Publish product-reservation (5ms)
│
└─ [InventoryWorker] Handle product-reservation (65ms)
   ├─ [DB] Reserve Inventory (30ms)
   └─ [Kafka] Publish product-reserved (10ms)
```

### Logging

**View Logs**:

**Aspire Dashboard**:
- Console tab shows all module logs
- Filter by module or log level

**Console Output**:
```bash
# View specific module logs
dotnet run --project src/Orders.SagaOrchestrator
```

**Structured Logging Example**:
```
[12:30:45 INF] Saga started for OrderId: 3fa85f64-5717-4562-b3fc-2c963f66afa6
[12:30:45 INF] Publishing product-reservation event for ProductId: 550e8400-e29b-41d4-a716-446655440000
[12:30:46 INF] Received product-reserved event with Status: Success
[12:30:46 INF] Publishing payment-pending event for Amount: 1899.99
```

### Metrics (Future)

**Planned Metrics**:
- Event processing latency
- Consumer lag
- Order success/failure rate
- Payment success rate
- API request rate

**Recommended Tools**:
- Prometheus for metrics collection
- Grafana for visualization

---

## Testing & Debugging

### Manual Testing

**Postman Collection**:
1. Import from Swagger: `http://localhost:<port>/swagger/v1/swagger.json`
2. Create environment with base URL
3. Execute requests

**Testing Checklist**:
- ✅ Create product with valid data
- ✅ Create product with invalid data (negative price)
- ✅ Create order with sufficient inventory
- ✅ Create order with insufficient inventory
- ✅ Deactivate product
- ✅ Verify events in KafkaUI

### Debugging Event Flow

**Problem**: Order stuck in Pending status

**Debugging Steps**:

1. **Check Outbox Table**
   ```sql
   SELECT * FROM orders.outbox_integration_events 
   WHERE processed_at IS NULL;
   ```
   - If events present: Outbox worker may be stopped
   - If empty: Events already published

2. **Check Kafka Topic**
   - KafkaUI → Topics → `order-created`
   - Verify message was published

3. **Check Consumer Lag**
   - KafkaUI → Consumer Groups → `order-saga-orchestrator`
   - High lag indicates processing delays

4. **Check Saga State**
   ```sql
   SELECT * FROM orders.saga_events 
   WHERE order_id = '<order-id>' 
   ORDER BY created_at;
   ```
   - Shows saga progression and where it stopped

5. **Check Logs**
   - Aspire Dashboard → Logs
   - Filter by OrderId
   - Look for errors or exceptions

---

## Production Considerations

### Deployment Checklist

- [ ] Configure production connection strings
- [ ] Set up Kafka cluster (3+ brokers)
- [ ] Enable Kafka replication (factor: 3)
- [ ] Configure consumer group auto-commit
- [ ] Set up database connection pooling
- [ ] Configure OpenTelemetry collector
- [ ] Set up log aggregation (ELK stack)
- [ ] Configure health checks
- [ ] Set up monitoring and alerting
- [ ] Implement rate limiting
- [ ] Add authentication/authorization
- [ ] Enable HTTPS
- [ ] Configure CORS policies
- [ ] Consider load balancing for the application

### Configuration Changes

**appsettings.Production.json**:
```json
{
  "ConnectionStrings": {
    "EdaMicroDb": "Host=prod-db.example.com;Port=5432;..."
  },
  "MessageBroker": {
    "BootstrapServers": "kafka-1:9092,kafka-2:9092,kafka-3:9092"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Error"
    }
  }
}
```

### Kafka Production Settings

```csharp
.AddConsumer(consumer => consumer
    .WithAutoCommitIntervalMs(5000)
    .WithEnableAutoCommit(true)
    .WithSessionTimeoutMs(30000)
    .WithMaxPollIntervalMs(300000)
    .WithWorkersCount(20) // Adjust based on load
);
```

### Database Optimization

**Indexes**:
```sql
-- Orders
CREATE INDEX idx_orders_status ON orders.orders(status);
CREATE INDEX idx_orders_customer ON orders.orders(customer_id);

-- Outbox
CREATE INDEX idx_outbox_unprocessed 
ON outbox_integration_events(processed_at) 
WHERE processed_at IS NULL;

-- Saga
CREATE INDEX idx_saga_order ON saga_events(order_id);
```

### Monitoring Production

**Key Metrics**:
- Consumer lag (alert if > 1000)
- Event processing duration (p95, p99)
- Order success rate (alert if < 95%)
- API latency (p95, p99)
- Database connection pool usage

**Alerting Rules**:
```yaml
- alert: HighConsumerLag
  expr: kafka_consumer_lag > 1000
  for: 5m
  
- alert: OrderFailureRate
  expr: order_failure_rate > 0.05
  for: 10m
```

---

## Tips & Best Practices

### Development Tips

1. **Use Aspire Dashboard**: Real-time view of all modules and traces
2. **KafkaUI for Events**: Verify messages are published/consumed correctly
3. **Database Queries**: Understand state changes during development
4. **Log Correlation IDs**: Trace requests across modules
5. **Clean Docker Volumes**: `docker volume prune` when testing from scratch

### Event Design Tips

1. **Include All Context**: Events should carry all data consumers need
2. **Use Strong Types**: Strongly-typed IDs prevent bugs
3. **Immutable Events**: Never modify published events
4. **Version Events**: Plan for schema evolution
5. **Small Events**: Keep payload size reasonable

### Saga Pattern Tips

1. **Compensation Logic**: Always implement for each step
2. **Timeout Handling**: Set reasonable timeouts for each step
3. **State Persistence**: Store saga state in database
4. **Monitoring**: Track saga completion rates
5. **Testing**: Test all failure scenarios

---

## Additional Resources

- **Architecture Details**: [architecture.md](architecture.md)
- **API Reference**: [api.md](api.md)
- **Contributing Guide**: [contributing.md](contributing.md)
- **Kafka Documentation**: [https://kafka.apache.org/documentation/](https://kafka.apache.org/documentation/)
- **.NET Aspire**: [https://learn.microsoft.com/dotnet/aspire/](https://learn.microsoft.com/dotnet/aspire/)
