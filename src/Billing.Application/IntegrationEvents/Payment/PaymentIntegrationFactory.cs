using System.Text.Json;
using Billing.Domain.Entities.Events;

namespace Billing.Application.IntegrationEvents.Payment;

public static class PaymentIntegrationFactory
{
    public static PaymentProcessedIntegrationEvent FromDomain(PaymentProcessedEvent evt)
    {
        var payload = JsonSerializer.Serialize(evt);
        return new PaymentProcessedIntegrationEvent(EventType.PaymentProcessed, payload);
    }    
}