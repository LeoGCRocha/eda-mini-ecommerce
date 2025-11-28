using MediatR;
using System.Text.Json;
using Billing.Domain.Entities.Events;
using Billing.Application.Observability;
using EdaMicroEcommerce.Application.Outbox;

namespace Billing.Application.IntegrationEvents.Payment;

public class PaymentProcessedIntegrationEventHandler(IIntegrationEventPublisher publisher) : IRequestHandler<PaymentProcessedIntegrationEvent>
{
    public async Task Handle(PaymentProcessedIntegrationEvent request, CancellationToken cancellationToken)
    {
        using var activity =
            Source.BillingSource.StartActivity(
                $"{nameof(PaymentProcessedIntegrationEventHandler)} : Sending message through broker.");

        var @object = JsonSerializer.Deserialize<PaymentProcessedEvent>(request.Payload);
        await publisher.PublishOnTopicAsync(@object, MessageBrokerConst.PaymentProcessedProducer, @object!.PaymentId.ToString());
    }
}