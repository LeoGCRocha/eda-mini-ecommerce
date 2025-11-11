using System.Diagnostics;
using System.Text;
using KafkaFlow;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using Orders.Application.Observability;
using Orders.Application.Saga;
using Orders.Domain.Entities.Events;

namespace Orders.Saga.IntegrationEvents;

public class OrderCreatedMessageHandler(ISagaOrchestrator sagaOrchestrator) : IMessageHandler<OrderCreatedEvent>
{
    private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;
    
    public async Task Handle(IMessageContext context, OrderCreatedEvent message)
    {
        var parentContext = Propagator.Extract(default, context.Headers, ExtractHeader);
        Baggage.Current = parentContext.Baggage;
        
        using var activity =
            Source.OrderSource.StartActivity(
                $"{nameof(OrderCreatedMessageHandler)} : Creating a new order",
                ActivityKind.Consumer, parentContext.ActivityContext);
        
        activity?.SetTag("message.system", "kafka");
        activity?.SetTag("messaging.operation", "consuming");
        activity?.SetTag("messaging.operation", "process");
        
        await sagaOrchestrator.ExecuteAsync(message.OrderId, message);
    }
    
    // TODO: Move to a common file
    private static IEnumerable<string> ExtractHeader(IMessageHeaders headers, string key)
    {
        return from header in headers
            where header.Key.Equals(key, StringComparison.OrdinalIgnoreCase)
            select Encoding.UTF8.GetString(header.Value);
    }
}