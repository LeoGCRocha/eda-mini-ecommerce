using EdaMicroEcommerce.Application.Outbox;

namespace Catalog.Application.IntegrationEvents.Products.ProductDeactivated;

public class ProductDeactivatedIntegration(EventType type, string payload)
    : OutboxIntegrationEvent<EventType>(type, payload);