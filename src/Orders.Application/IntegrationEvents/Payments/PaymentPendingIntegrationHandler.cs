using MediatR;
using System.Text.Json;
using EdaMicroEcommerce.Application.Outbox;
using Platform.SharedContracts.IntegrationEvents.Payments;

namespace Orders.Application.IntegrationEvents.Payments;

public class PaymentPendingIntegrationHandler(IIntegrationEventPublisher publisher) : IRequestHandler<PaymentPendingIntegration>
{
    public async Task Handle(PaymentPendingIntegration request, CancellationToken cancellationToken)
    {
        var @object = JsonSerializer.Deserialize<PaymentPendingEvent>(request.Payload);
        await publisher.PublishOnTopicAsync(@object, MessageBrokerConst.PaymentPendingProducer);
    }
}