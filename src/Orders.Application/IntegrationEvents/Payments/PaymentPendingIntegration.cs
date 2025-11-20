using EdaMicroEcommerce.Application.Outbox;

namespace Orders.Application.IntegrationEvents.Payments;

public class PaymentPendingIntegration(EventType type, string payload)
    : OutboxIntegrationEvent<EventType>(type, payload);