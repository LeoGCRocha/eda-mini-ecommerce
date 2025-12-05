# API Documentation

## Table of Contents
1. [API Overview](#api-overview)
2. [Authentication](#authentication)
3. [Common Response Formats](#common-response-formats)
4. [Orders API](#orders-api)
5. [Catalog API](#catalog-api)
6. [Billing API](#billing-api)
7. [Error Handling](#error-handling)
8. [Rate Limiting](#rate-limiting)

---

## API Overview

### Base URLs

When running locally with .NET Aspire, each service gets a dynamic URL. Check the Aspire Dashboard for actual URLs.

**Typical Local URLs:**
- **Main API Gateway**: `http://localhost:<dynamic-port>`
- **Swagger UI**: Available at `http://localhost:<port>/swagger` in Development mode

### API Versioning

All endpoints are versioned using URL path versioning:
```
/api/v1/{resource}
```

Current version: **v1**

### Content Type

All requests and responses use JSON:
```
Content-Type: application/json
```

### Request ID Tracing

All responses include a trace identifier for debugging:
```
X-Trace-Id: 3fa85f64-5717-4562-b3fc-2c963f66afa6
```

---

## Authentication

**Current Status**: No authentication implemented (development mode)

**Future Implementation**: JWT Bearer tokens
```http
Authorization: Bearer <token>
```

---

## Common Response Formats

### Success Response (200 OK)
```json
{
  "data": { ... },
  "timestamp": "2024-12-05T10:30:00Z"
}
```

### Created Response (201 Created)
```http
HTTP/1.1 201 Created
Location: /api/v1/orders/3fa85f64-5717-4562-b3fc-2c963f66afa6

{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

### No Content Response (204 No Content)
```http
HTTP/1.1 204 No Content
```

### Error Response (4xx, 5xx)
See [Error Handling](#error-handling) section.

---

## Orders API

### Create Order

Creates a new order and initiates the order processing workflow.

**Endpoint**: `POST /api/v1/order`

**Request Body**:
```json
{
  "customerId": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
  "orderItemDtos": [
    {
      "productId": "550e8400-e29b-41d4-a716-446655440000",
      "quantity": 2
    },
    {
      "productId": "6ba7b810-9dad-11d1-80b4-00c04fd430c8",
      "quantity": 1
    }
  ]
}
```

**Request Fields**:

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `customerId` | string (UUID) | Yes | Unique customer identifier |
| `orderItemDtos` | array | Yes | List of products to order |
| `orderItemDtos[].productId` | string (UUID) | Yes | Product identifier |
| `orderItemDtos[].quantity` | integer | Yes | Quantity to order (must be > 0) |

**Success Response**: `204 No Content`

**Side Effects**:
1. Order created in database with `Pending` status
2. `order-created` event published to Kafka
3. Saga orchestrator begins workflow

**Example cURL**:
```bash
curl -X POST http://localhost:5000/api/v1/order \
  -H "Content-Type: application/json" \
  -d '{
    "customerId": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
    "orderItemDtos": [
      {
        "productId": "550e8400-e29b-41d4-a716-446655440000",
        "quantity": 2
      }
    ]
  }'
```

**Validation Rules**:
- `customerId` must be a valid UUID
- `orderItemDtos` cannot be empty
- `quantity` must be greater than 0
- `productId` must be a valid UUID (product existence validated asynchronously)

**Error Responses**:

**400 Bad Request** - Invalid input
```json
{
  "type": "ValidationException",
  "status": 400,
  "title": "Validation failed",
  "detail": "One or more validation errors occurred.",
  "errors": {
    "orderItemDtos": ["Order must contain at least one item"]
  }
}
```

**500 Internal Server Error** - Domain exception
```json
{
  "type": "DomainException",
  "status": 500,
  "title": "Something bad happens.",
  "detail": "Cannot create order for inactive customer",
  "instance": "/api/v1/order"
}
```

---

## Catalog API

### Create Product

Adds a new product to the catalog with initial inventory.

**Endpoint**: `POST /api/v1/product`

**Request Body**:
```json
{
  "name": "Wireless Mouse",
  "description": "Ergonomic wireless mouse with USB receiver",
  "price": 29.99,
  "availableQuantity": 100,
  "reorderQuantity": 20
}
```

**Request Fields**:

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `name` | string | Yes | Product name (max 255 chars) |
| `description` | string | Yes | Product description |
| `price` | decimal | Yes | Unit price (must be > 0) |
| `availableQuantity` | integer | Yes | Initial stock quantity |
| `reorderQuantity` | integer | Yes | Threshold for low stock alerts |

**Success Response**: `200 OK`

**Example cURL**:
```bash
curl -X POST http://localhost:5000/api/v1/product \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Wireless Mouse",
    "description": "Ergonomic wireless mouse",
    "price": 29.99,
    "availableQuantity": 100,
    "reorderQuantity": 20
  }'
```

**Side Effects**:
1. Product created in catalog
2. Inventory item initialized with available quantity

**Error Responses**:

**400 Bad Request** - Invalid input
```json
{
  "type": "ValidationException",
  "status": 400,
  "title": "Validation failed",
  "detail": "Price must be greater than zero"
}
```

---

### Deactivate Product

Marks a product as inactive, preventing new orders.

**Endpoint**: `POST /api/v1/product/{id}/deactivate`

**Path Parameters**:

| Parameter | Type | Description |
|-----------|------|-------------|
| `id` | UUID | Product identifier |

**Request Body**: None

**Success Response**: `200 OK`

**Example cURL**:
```bash
curl -X POST http://localhost:5000/api/v1/product/550e8400-e29b-41d4-a716-446655440000/deactivate
```

**Side Effects**:
1. Product marked as inactive
2. `product-deactivated` event published to Kafka
3. Inventory worker updates stock status

**Error Responses**:

**404 Not Found** - Product doesn't exist
```json
{
  "type": "NotFoundException",
  "status": 404,
  "title": "Product not found",
  "detail": "Product with id 550e8400-e29b-41d4-a716-446655440000 does not exist"
}
```

---

## Billing API

### Process Payment

Manually triggers payment processing for a payment record (development/testing endpoint).

**Endpoint**: `PATCH /api/v1/payment/{id}/process`

**Path Parameters**:

| Parameter | Type | Description |
|-----------|------|-------------|
| `id` | UUID | Payment identifier |

**Request Body**:
```json
{
  "couponName": "DISCOUNT10"
}
```

**Request Fields**:

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `couponName` | string | No | Optional discount coupon code |

**Success Response**: `200 OK`
```json
{
  "paymentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "status": "Processed",
  "amount": 89.99,
  "discountApplied": 10.00,
  "finalAmount": 79.99
}
```

**Example cURL**:
```bash
curl -X PATCH http://localhost:5000/api/v1/payment/3fa85f64-5717-4562-b3fc-2c963f66afa6/process \
  -H "Content-Type: application/json" \
  -d '{
    "couponName": "DISCOUNT10"
  }'
```

**Side Effects**:
1. Payment status updated
2. Discount applied if coupon valid

**Note**: In production, payment processing is triggered automatically by the `Billing.PaymentWorker` consuming `payment-pending` events. This endpoint is for manual testing only.

**Error Responses**:

**404 Not Found** - Payment doesn't exist
```json
{
  "type": "NotFoundException",
  "status": 404,
  "title": "Payment not found",
  "detail": "Payment with id 3fa85f64-5717-4562-b3fc-2c963f66afa6 does not exist"
}
```

**400 Bad Request** - Invalid payment state
```json
{
  "type": "DomainException",
  "status": 400,
  "title": "Invalid operation",
  "detail": "Payment already processed"
}
```

---

## Error Handling

### Problem Details Format (RFC 7807)

All errors follow the Problem Details specification:

```json
{
  "type": "string",       // Error type identifier
  "status": 500,          // HTTP status code
  "title": "string",      // Short error summary
  "detail": "string",     // Detailed error message
  "instance": "string"    // Request path that caused the error
}
```

### Error Types

| Type | Status Code | Description |
|------|-------------|-------------|
| `ValidationException` | 400 | Request validation failed |
| `NotFoundException` | 404 | Resource not found |
| `DomainException` | 500 | Business rule violation |
| `GenericException` | 500 | Unexpected error |

### Common Error Scenarios

#### 400 Bad Request
**When**: Invalid request payload or validation failure

**Example**:
```json
{
  "type": "ValidationException",
  "status": 400,
  "title": "Validation failed",
  "detail": "One or more validation errors occurred.",
  "instance": "/api/v1/order",
  "errors": {
    "customerId": ["The customerId field is required."],
    "orderItemDtos[0].quantity": ["Quantity must be greater than 0"]
  }
}
```

#### 404 Not Found
**When**: Resource doesn't exist

**Example**:
```json
{
  "type": "NotFoundException",
  "status": 404,
  "title": "Resource not found",
  "detail": "Order with id 3fa85f64-5717-4562-b3fc-2c963f66afa6 not found",
  "instance": "/api/v1/order/3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

#### 500 Internal Server Error
**When**: Domain rule violation or unexpected error

**Example**:
```json
{
  "type": "DomainException",
  "status": 500,
  "title": "Something bad happens.",
  "detail": "Cannot modify order in Confirmed status",
  "instance": "/api/v1/order/3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

---

## Rate Limiting

**Current Status**: Not implemented

**Future Implementation**: Token bucket algorithm
- 100 requests per minute per client
- 1000 requests per hour per client

**Headers** (when implemented):
```
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 95
X-RateLimit-Reset: 1638360000
```

---

## API Testing

### Swagger UI

Access interactive API documentation at:
```
http://localhost:<port>/swagger
```

**Features**:
- Try endpoints directly from browser
- View request/response schemas
- Generate code samples

### Postman Collection

Import the OpenAPI specification from Swagger:
```
http://localhost:<port>/swagger/v1/swagger.json
```

### Example Workflow: Complete Order

**Step 1: Create Product**
```bash
curl -X POST http://localhost:5000/api/v1/product \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Laptop",
    "description": "15-inch laptop",
    "price": 999.99,
    "availableQuantity": 50,
    "reorderQuantity": 10
  }'
```

**Step 2: Create Order**
```bash
curl -X POST http://localhost:5000/api/v1/order \
  -H "Content-Type: application/json" \
  -d '{
    "customerId": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
    "orderItemDtos": [
      {
        "productId": "<product-id-from-step-1>",
        "quantity": 1
      }
    ]
  }'
```

**Step 3: Monitor Events**

Check KafkaUI at `http://localhost:9100` to see:
1. `order-created` event
2. `product-reservation` event
3. `product-reserved` event
4. `payment-pending` event
5. `payment-processed` event

**Step 4: Verify Order Status**

Check database or logs to confirm order moved from `Pending` to `Confirmed` status.

---

## Webhooks (Future)

**Planned**: Webhook notifications for async events

**Example Events**:
- `order.created`
- `order.confirmed`
- `order.cancelled`
- `payment.processed`
- `inventory.low_stock`

**Webhook Payload**:
```json
{
  "event": "order.confirmed",
  "timestamp": "2024-12-05T10:30:00Z",
  "data": {
    "orderId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "status": "Confirmed"
  }
}
```

---

## API Versioning Strategy

### Current: URL Path Versioning
```
/api/v1/order
/api/v2/order  (future)
```

### Deprecation Policy
- Old versions supported for 6 months after new version release
- Deprecation warnings in response headers:
  ```
  Deprecated: true
  Sunset: Fri, 01 Dec 2024 00:00:00 GMT
  ```

---

## Additional Resources

- **Architecture**: See [architecture.md](architecture.md) for system design
- **Usage Examples**: See [usage.md](usage.md) for detailed scenarios
- **Contributing**: See [contributing.md](contributing.md) for API development guidelines
