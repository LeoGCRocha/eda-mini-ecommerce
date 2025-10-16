using KafkaFlow;
using Orders.Application.Saga;
using Orders.Domain.Entities.Events;

namespace Orders.Saga.IntegrationEvents;

public class OrderCreatedMessageHandler : IMessageHandler<OrderCreatedEvent>
{
    private readonly ISagaOrchestrator _sagaOrchestrator;

    public OrderCreatedMessageHandler(ISagaOrchestrator sagaOrchestrator)
    {
        _sagaOrchestrator = sagaOrchestrator;
    }

    public async Task Handle(IMessageContext context, OrderCreatedEvent message)
    {
        await _sagaOrchestrator.ExecuteAsync(message.OrderId, message);
    }
}