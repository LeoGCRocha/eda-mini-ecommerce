using EdaMicroEcommerce.Application.Outbox;

namespace Orders.Application.IntegrationEvents.Orders;

public class OrderCreatedIntegration(EventType type, string payload)
    : OutboxIntegrationEvent<EventType>(type, payload);