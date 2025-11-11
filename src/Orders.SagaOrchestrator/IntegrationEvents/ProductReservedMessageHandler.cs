using KafkaFlow;
using System.Text;
using OpenTelemetry;
using System.Diagnostics;
using Orders.Application.Saga;
using Orders.Domain.Entities.Events;
using Orders.Application.Observability;
using OpenTelemetry.Context.Propagation;

namespace Orders.Saga.IntegrationEvents;

public class ProductReservedMessageHandler(ISagaOrchestrator sagaOrchestrator)
    : IMessageHandler<ProductReservationStatusEvent>
{
    private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;
    public async Task Handle(IMessageContext context, ProductReservationStatusEvent message)
    {
        var parentContext = Propagator.Extract(default, context.Headers, ExtractHeader);
        Baggage.Current = parentContext.Baggage;
        
        using var activity =
            Source.OrderSource.StartActivity(
                $"{nameof(ProductReservedMessageHandler)} : Receive a reservation update to the order",
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