# Contributing Guide

## Table of Contents
1. [Getting Started](#getting-started)
2. [Development Workflow](#development-workflow)
3. [Code Standards](#code-standards)
4. [Architecture Guidelines](#architecture-guidelines)
5. [Testing Requirements](#testing-requirements)
6. [Pull Request Process](#pull-request-process)
7. [Event-Driven Development Patterns](#event-driven-development-patterns)

---

## Getting Started

### Development Environment Setup

**Required Tools**:
- .NET SDK 8.0.415 or later
- Docker Desktop (for Kafka and PostgreSQL)
- Git
- IDE: Visual Studio 2022, JetBrains Rider, or VS Code

**Optional but Recommended**:
- Kafka CLI tools
- PostgreSQL client (pgAdmin, DBeaver)
- Postman or similar API testing tool

**Setup Steps**:

1. **Fork the Repository**
   ```bash
   # Fork on GitHub, then clone your fork
   git clone https://github.com/YOUR_USERNAME/eda-mini-ecommerce.git
   cd eda-mini-ecommerce
   ```

2. **Add Upstream Remote**
   ```bash
   git remote add upstream https://github.com/LeoGCRocha/eda-mini-ecommerce.git
   ```

3. **Install Dependencies**
   ```bash
   dotnet restore
   ```

4. **Run the Application**
   ```bash
   dotnet run --project src/EdaMicroEcommerce.AppHost/EdaMicroEcommerce.AppHost.csproj
   ```

5. **Verify Setup**
   - Aspire Dashboard opens at `http://localhost:15000`
   - All services show as "Running"
   - KafkaUI accessible at `http://localhost:9100`

---

## Development Workflow

### Branching Strategy

**Branch Naming Convention**:
```
feature/<feature-name>      # New features
bugfix/<bug-description>    # Bug fixes
hotfix/<critical-fix>       # Production hotfixes
refactor/<refactor-name>    # Code refactoring
docs/<documentation-change> # Documentation updates
```

**Examples**:
- `feature/add-order-cancellation`
- `bugfix/inventory-reservation-race-condition`
- `refactor/extract-saga-base-class`

### Development Process

1. **Create Feature Branch**
   ```bash
   git checkout -b feature/my-new-feature
   ```

2. **Make Changes**
   - Follow [Code Standards](#code-standards)
   - Write tests
   - Update documentation if needed

3. **Test Locally**
   ```bash
   # Run all tests
   dotnet test
   
   # Run specific test project
   dotnet test tests/Orders.Domain.Tests
   ```

4. **Commit Changes**
   ```bash
   git add .
   git commit -m "feat: add order cancellation endpoint"
   ```

5. **Keep Branch Updated**
   ```bash
   git fetch upstream
   git rebase upstream/main
   ```

6. **Push to Fork**
   ```bash
   git push origin feature/my-new-feature
   ```

7. **Create Pull Request**
   - Go to GitHub
   - Create PR from your fork to upstream main
   - Fill out PR template

---

## Code Standards

### C# Coding Conventions

Follow the [C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions).

**Key Rules**:

1. **Naming Conventions**
   ```csharp
   // Classes, methods, properties: PascalCase
   public class OrderService { }
   public void ProcessOrder() { }
   public string CustomerName { get; set; }
   
   // Local variables, parameters: camelCase
   var customerId = Guid.NewGuid();
   public void CreateOrder(string orderId) { }
   
   // Private fields: _camelCase
   private readonly IOrderRepository _orderRepository;
   
   // Interfaces: IPascalCase
   public interface IOrderRepository { }
   ```

2. **File Organization**
   ```csharp
   // 1. Usings (sorted)
   using System;
   using System.Linq;
   using EdaMicroEcommerce.Domain;
   
   // 2. Namespace
   namespace Orders.Application;
   
   // 3. Class definition
   public class OrderService
   {
       // Fields
       private readonly IOrderRepository _repository;
       
       // Constructor
       public OrderService(IOrderRepository repository)
       {
           _repository = repository;
       }
       
       // Properties
       public int OrderCount { get; private set; }
       
       // Methods
       public async Task<Order> CreateOrderAsync(CreateOrderCommand command)
       {
           // Implementation
       }
   }
   ```

3. **Use Modern C# Features**
   ```csharp
   // ✅ Good: Record for DTOs
   public record CreateOrderCommand(string CustomerId, List<OrderItemDto> Items);
   
   // ✅ Good: Pattern matching
   var status = order.Status switch
   {
       OrderStatus.Pending => "Processing",
       OrderStatus.Confirmed => "Confirmed",
       _ => "Unknown"
   };
   
   // ✅ Good: Null-conditional operators
   var customerName = customer?.Name ?? "Unknown";
   
   // ✅ Good: File-scoped namespaces (C# 10+)
   namespace Orders.Domain;
   
   public class Order { }
   ```

4. **Async/Await Best Practices**
   ```csharp
   // ✅ Good: Async all the way
   public async Task<Order> GetOrderAsync(OrderId id)
   {
       return await _repository.GetByIdAsync(id);
   }
   
   // ❌ Bad: Blocking on async code
   public Order GetOrder(OrderId id)
   {
       return _repository.GetByIdAsync(id).Result; // Don't do this
   }
   ```

### Project Structure

**Follow DDD Layering**:
```
src/
├── {BoundedContext}.Domain/       # Entities, Value Objects, Domain Events
│   ├── Entities/
│   ├── ValueObjects/
│   └── Events/
├── {BoundedContext}.Application/  # Use Cases, Command/Query Handlers
│   ├── Commands/
│   ├── Queries/
│   └── IntegrationEvents/
├── {BoundedContext}.Infra/        # EF Core, Repositories, Outbox
│   ├── Persistence/
│   ├── Repositories/
│   └── Outbox/
└── {BoundedContext}.Api/          # HTTP Endpoints
    └── CQS/
```

**File Naming**:
- One class per file
- File name matches class name
- Example: `Order.cs` contains `public class Order`

---

## Architecture Guidelines

### Domain-Driven Design Principles

1. **Aggregate Boundaries**
   - Keep aggregates small
   - Reference other aggregates by ID only
   - Enforce invariants within aggregate

   ```csharp
   // ✅ Good: Reference by ID
   public class Order
   {
       public CustomerId CustomerId { get; private set; }
   }
   
   // ❌ Bad: Direct reference to other aggregate
   public class Order
   {
       public Customer Customer { get; private set; } // Don't do this
   }
   ```

2. **Domain Events**
   - Events are immutable facts
   - Named in past tense
   - Contain all necessary data

   ```csharp
   // ✅ Good: Past tense, immutable
   public class OrderCreatedEvent : IDomainEvent
   {
       public OrderId OrderId { get; init; }
       public DateTime CreatedAt { get; init; }
       
       public OrderCreatedEvent(OrderId orderId, DateTime createdAt)
       {
           OrderId = orderId;
           CreatedAt = createdAt;
       }
   }
   ```

3. **Repository Pattern**
   - One repository per aggregate root
   - Only aggregate roots have repositories

   ```csharp
   public interface IOrderRepository
   {
       Task<Order> GetByIdAsync(OrderId id);
       Task AddAsync(Order order);
       Task UpdateAsync(Order order);
   }
   ```

### CQRS Implementation

**Commands** (Write Operations):
```csharp
// Command
public record CreateOrderCommand(string CustomerId, List<OrderItemDto> Items) 
    : IRequest;

// Handler
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand>
{
    public async Task Handle(CreateOrderCommand request, CancellationToken ct)
    {
        // Validate, create entity, raise events, save
    }
}
```

**Queries** (Read Operations):
```csharp
// Query
public record GetOrderQuery(Guid OrderId) : IRequest<OrderDto>;

// Handler
public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, OrderDto>
{
    public async Task<OrderDto> Handle(GetOrderQuery request, CancellationToken ct)
    {
        // Fetch and map to DTO
    }
}
```

### Event-Driven Patterns

**Outbox Pattern** (always use for domain events):
```csharp
using (var transaction = await _dbContext.Database.BeginTransactionAsync())
{
    // 1. Business logic
    _dbContext.Orders.Add(order);
    
    // 2. Store event in outbox
    _dbContext.OutboxEvents.Add(new OutboxEvent
    {
        EventType = "order-created",
        Payload = JsonSerializer.Serialize(orderCreatedEvent),
        CreatedAt = DateTime.UtcNow
    });
    
    await _dbContext.SaveChangesAsync();
    await transaction.CommitAsync();
}
```

**Consumer Idempotency** (always implement):
```csharp
public async Task Handle(IMessageContext context, OrderCreatedEvent message)
{
    var eventId = $"order-created-{message.OrderId.Value}";
    
    if (await _dbContext.ProcessedEvents.AnyAsync(e => e.EventId == eventId))
        return; // Already processed
    
    // Process event...
    
    _dbContext.ProcessedEvents.Add(new ProcessedEvent { EventId = eventId });
    await _dbContext.SaveChangesAsync();
}
```

---

## Testing Requirements

### Unit Tests

**Location**: `tests/{BoundedContext}.Domain.Tests/`

**What to Test**:
- Domain entity behavior
- Business rule validation
- Value object equality
- Domain event raising

**Example**:
```csharp
public class OrderTests
{
    [Fact]
    public void AddItem_WithValidProduct_AddsToOrder()
    {
        // Arrange
        var order = new Order(new CustomerId(Guid.NewGuid()));
        var productId = new ProductId(Guid.NewGuid());
        
        // Act
        order.AddItem(productId, quantity: 2, price: 10.00m);
        
        // Assert
        Assert.Single(order.Items);
        Assert.Equal(2, order.Items.First().Quantity);
    }
    
    [Fact]
    public void AddItem_WithZeroQuantity_ThrowsDomainException()
    {
        // Arrange
        var order = new Order(new CustomerId(Guid.NewGuid()));
        
        // Act & Assert
        Assert.Throws<DomainException>(() => 
            order.AddItem(new ProductId(Guid.NewGuid()), quantity: 0, price: 10.00m)
        );
    }
}
```

### Integration Tests

**Location**: `tests/{BoundedContext}.Integration.Tests/`

**What to Test**:
- API endpoints
- Database operations
- Event publishing to Kafka
- Complete workflows

**Example**:
```csharp
public class OrderApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    
    public OrderApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }
    
    [Fact]
    public async Task CreateOrder_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var client = _factory.CreateClient();
        var command = new CreateOrderCommand(
            CustomerId: Guid.NewGuid().ToString(),
            Items: new List<OrderItemDto> 
            { 
                new(Guid.NewGuid().ToString(), 2) 
            }
        );
        
        // Act
        var response = await client.PostAsJsonAsync("/api/v1/order", command);
        
        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
```

### Event Testing

**Test Event Publishing**:
```csharp
[Fact]
public async Task CreateOrder_PublishesOrderCreatedEvent()
{
    // Arrange
    var order = new Order(new CustomerId(Guid.NewGuid()));
    
    // Act
    order.AddItem(new ProductId(Guid.NewGuid()), 1, 10m);
    
    // Assert
    var domainEvent = order.GetDomainEvents().OfType<OrderCreatedEvent>().Single();
    Assert.Equal(order.Id, domainEvent.OrderId);
}
```

**Test Event Consumption** (use TestContainers for Kafka):
```csharp
[Fact]
public async Task OrderCreatedHandler_CreatesaSagaEvent()
{
    // Arrange
    var handler = new OrderCreatedMessageHandler(_dbContext, _publisher);
    var message = new OrderCreatedEvent(new OrderId(Guid.NewGuid()));
    
    // Act
    await handler.Handle(_messageContext, message);
    
    // Assert
    var saga = await _dbContext.SagaEvents
        .FirstOrDefaultAsync(s => s.OrderId == message.OrderId);
    Assert.NotNull(saga);
}
```

### Test Coverage Goals

- **Domain Layer**: 80%+ coverage
- **Application Layer**: 70%+ coverage
- **API Layer**: 60%+ coverage

Run coverage:
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

---

## Pull Request Process

### PR Checklist

Before submitting a PR, ensure:

- [ ] Code follows style guidelines
- [ ] All tests pass locally
- [ ] New tests added for new features
- [ ] Documentation updated (if needed)
- [ ] No merge conflicts with main
- [ ] Commit messages follow convention
- [ ] PR description is clear

### Commit Message Convention

Follow [Conventional Commits](https://www.conventionalcommits.org/):

```
<type>(<scope>): <subject>

<body>

<footer>
```

**Types**:
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `refactor`: Code refactoring
- `test`: Adding tests
- `chore`: Maintenance tasks

**Examples**:
```
feat(orders): add order cancellation endpoint

Implements the ability for users to cancel pending orders.
Includes compensation logic to release reserved inventory.

Closes #123

---

fix(catalog): resolve inventory reservation race condition

Add database-level locking to prevent double-booking inventory
when multiple orders arrive simultaneously.

Fixes #456

---

docs(readme): update setup instructions for macOS

Add specific commands for installing Docker Desktop on macOS
and troubleshooting common installation issues.
```

### PR Review Process

1. **Create PR** with detailed description
2. **Automated Checks** run (build, tests)
3. **Code Review** by maintainers
4. **Address Feedback** if requested
5. **Approval** from at least one maintainer
6. **Merge** to main branch

### Review Criteria

**Reviewers Check**:
- Code quality and readability
- Adherence to architecture patterns
- Test coverage
- Performance implications
- Security considerations
- Breaking changes

---

## Event-Driven Development Patterns

### Adding a New Event

1. **Define Integration Event**
   ```csharp
   // In Platform.SharedContracts/IntegrationEvents/
   public class OrderCancelledEvent
   {
       public OrderId OrderId { get; init; }
       public string Reason { get; init; }
       public DateTime CancelledAt { get; init; }
   }
   ```

2. **Publish Event in Domain**
   ```csharp
   public class Order
   {
       public void Cancel(string reason)
       {
           Status = OrderStatus.Cancelled;
           RaiseDomainEvent(new OrderCancelledEvent(Id, reason, DateTime.UtcNow));
       }
   }
   ```

3. **Configure Kafka Topic**
   ```csharp
   // In worker Program.cs
   .CreateTopicIfNotExists("order-cancelled", numPartitions: 3, replicationFactor: 1)
   ```

4. **Add Consumer Handler**
   ```csharp
   public class OrderCancelledHandler : IMessageHandler<OrderCancelledEvent>
   {
       public async Task Handle(IMessageContext context, OrderCancelledEvent message)
       {
           // Handle event (e.g., send notification)
       }
   }
   ```

5. **Register Handler**
   ```csharp
   .AddTypedHandlers(handlers => handlers
       .AddHandler<OrderCancelledHandler>()
   )
   ```

### Adding a New Consumer Service

See [Usage Guide - Scenario 2](usage.md#scenario-2-adding-a-new-event-subscriber) for detailed steps.

---

## Best Practices

### Performance

- Use `AsNoTracking()` for read-only queries
- Implement database indexes for frequent queries
- Use pagination for large result sets
- Configure Kafka consumer `WorkersCount` based on load

### Security

- Never log sensitive data (credit cards, passwords)
- Validate all input
- Use parameterized queries (EF Core does this automatically)
- Implement authentication/authorization before production

### Observability

- Use structured logging
- Include correlation IDs in logs
- Add OpenTelemetry tracing for new operations
- Log important business events (not just errors)

### Documentation

- Update README.md for major features
- Add XML comments for public APIs
- Update architecture diagrams if needed
- Document breaking changes

---

## Getting Help

**Questions or Issues?**

1. Check [existing issues](https://github.com/LeoGCRocha/eda-mini-ecommerce/issues)
2. Search [documentation](../README.md)
3. Create a new issue with:
   - Clear description
   - Steps to reproduce (if bug)
   - Expected vs actual behavior
   - Environment details

**Discussion**:
- Use GitHub Discussions for questions
- Tag with appropriate labels

---

## Code of Conduct

- Be respectful and inclusive
- Provide constructive feedback
- Focus on the code, not the person
- Help newcomers learn

---

## Recognition

Contributors will be listed in:
- README.md Contributors section
- Release notes for their contributions

Thank you for contributing! 🎉
