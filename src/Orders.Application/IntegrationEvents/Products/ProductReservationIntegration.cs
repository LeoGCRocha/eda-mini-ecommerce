using EdaMicroEcommerce.Application.Outbox;

namespace Orders.Application.IntegrationEvents.Products;

public class ProductReservationIntegration(EventType type, string payload)
    : OutboxIntegrationEvent<EventType>(type, payload);