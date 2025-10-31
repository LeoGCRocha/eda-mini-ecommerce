using EdaMicroEcommerce.Application.Outbox;

namespace Catalog.Application.IntegrationEvents.Products.ProductDeactivated;

public class ProductReservedIntegration(EventType type, string payload)
    : OutboxIntegrationEvent<EventType>(type, payload);