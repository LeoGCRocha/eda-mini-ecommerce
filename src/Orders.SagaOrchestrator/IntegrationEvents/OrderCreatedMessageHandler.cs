using KafkaFlow;
using System.Diagnostics;
using Orders.Application.Saga;
using Orders.Domain.Entities.Events;

namespace Orders.Saga.IntegrationEvents;

public class OrderCreatedMessageHandler(ISagaOrchestrator sagaOrchestrator) : IMessageHandler<OrderCreatedEvent>
{
    public async Task Handle(IMessageContext context, OrderCreatedEvent message)
    {
        var activity = Activity.Current;
        
        activity?.SetTag("message.system", "kafka");
        activity?.SetTag("messaging.operation", "consuming");
        activity?.SetTag("messaging.operation", "process");
        
        await sagaOrchestrator.ExecuteAsync(message.OrderId, message);
    }
}