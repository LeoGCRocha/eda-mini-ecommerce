using KafkaFlow;
using Orders.Application.Saga;
using Orders.Domain.Entities.Events;

namespace Orders.Saga.IntegrationEvents;

public class ProductReservedMessageHandler : IMessageHandler<ProductReservationStatusEvent>
{
    private readonly ISagaOrchestrator _sagaOrchestrator;

    public ProductReservedMessageHandler(ISagaOrchestrator sagaOrchestrator)
    {
        _sagaOrchestrator = sagaOrchestrator;
    }

    public async Task Handle(IMessageContext context, ProductReservationStatusEvent message)
    {
        await _sagaOrchestrator.ExecuteAsync(message.OrderId, message);
    }
}