using EdaMicroEcommerce.Application.Outbox;

namespace Billing.Application.IntegrationEvents.Payment;

public class PaymentProcessedIntegrationEvent(EventType type, string payload)
 : OutboxIntegrationEvent<EventType>(type, payload);