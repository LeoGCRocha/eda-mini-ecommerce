using System.Diagnostics;
using System.Text;
using Catalog.Domain.Entities;
using Catalog.Domain.Entities.Products.Events;
using KafkaFlow;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace EcaMicroEcommerce.ProductWorker.IntegrationsEvent.ProductDeactivated;

public class ProductDeactivatedMessageHandler(
    ILogger<ProductDeactivatedMessageHandler> logger,
    IProductInventoryService productInventoryService)
    : IMessageHandler<ProductDeactivatedEvent>
{
    private static readonly ActivitySource Activity = new(nameof(ProductDeactivatedMessageHandler));
    private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;
    
    public async Task Handle(IMessageContext context, ProductDeactivatedEvent message)
    {
        // <WARNING....>
        // TODO: Preciso lidar com isso aqui tambem no SAGA....
        // TODO:Broker ta down nos workers....
        try
        {
            var parentContext = Propagator.Extract(default, context.Headers, ExtractHeader);
            Baggage.Current = parentContext.Baggage;

            using var activity =
                Activity.StartActivity("Processing message", ActivityKind.Consumer, parentContext.ActivityContext);

            activity?.SetTag("message.system", "kafka");
            activity?.SetTag("messaging.operation", "consuming");
            activity?.SetTag("messaging.operation", "process");

            var stopWatch = Stopwatch.StartNew();
            await productInventoryService.DeactivateProductOnInventoryAsync(message.ProductId);
            stopWatch.Stop();

            activity?.SetTag("processing.duration.ms", stopWatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Something bad happens during messaging consuming.");
            throw;
        }
    }

    private static IEnumerable<string> ExtractHeader(IMessageHeaders headers, string key)
    {
        return from header in headers where header.Key.Equals(key, StringComparison.OrdinalIgnoreCase) select Encoding.UTF8.GetString(header.Value);
    }
}