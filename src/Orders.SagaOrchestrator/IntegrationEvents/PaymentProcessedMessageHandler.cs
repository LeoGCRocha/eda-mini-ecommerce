using KafkaFlow;
using Orders.Application.Saga;
using Platform.SharedContracts.IntegrationEvents.Payments;

namespace Orders.Saga.IntegrationEvents;

public class PaymentProcessedMessageHandler(ISagaOrchestrator sagaOrchestrator)
    : IMessageHandler<PaymentProcessedEvent>
{
    public async Task Handle(IMessageContext context, PaymentProcessedEvent message)
    {
        await sagaOrchestrator.ExecuteAsync(message.OrderId, message);
    }
}