using EdaMicroEcommerce.Application.Outbox;

namespace EdaMicroEcommerce.Application.IntegrationEvents;

public class ProductDeactivationIntegration(string type, string payload)
    : OutboxIntegrationEvent(type, payload);