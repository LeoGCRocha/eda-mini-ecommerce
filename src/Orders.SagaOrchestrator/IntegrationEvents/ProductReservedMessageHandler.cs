using KafkaFlow;
using Orders.Application.Saga;
using Orders.Domain.Entities.Events;

namespace Orders.Saga.IntegrationEvents;

public class ProductReservedMessageHandler(ISagaOrchestrator sagaOrchestrator)
    : IMessageHandler<ProductReservationStatusEvent>
{
    public async Task Handle(IMessageContext context, ProductReservationStatusEvent message)
    {
        await sagaOrchestrator.ExecuteAsync(message.OrderId, message);
    }
}