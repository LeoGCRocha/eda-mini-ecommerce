using EdaMicroEcommerce.Application.Outbox;

namespace EdaMicroEcommerce.Application.IntegrationEvents.Products.ProductDeactivated;

// <WARNING> Uma possível mudança do nome da classe poderia impactar em tempo de execução a forma que o outbox message worker lida
public class ProductDeactivatedIntegration(EventType type, string payload)
    : OutboxIntegrationEvent(type, payload);