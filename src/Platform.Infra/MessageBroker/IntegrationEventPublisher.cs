using OpenTelemetry;
using System.Diagnostics;
using System.Text;
using KafkaFlow.Producers;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Context.Propagation;
using EdaMicroEcommerce.Application.Outbox;
using KafkaFlow;

namespace EdaMicroEcommerce.Infra.MessageBroker;

public class IntegrationEventPublisher(IProducerAccessor producerAccessor, ILogger<IntegrationEventPublisher> logger)
    : IIntegrationEventPublisher
{
    private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;

    public async Task PublishOnTopicAsync<T>(T payload, string producerName, string? key = null)
    {
        try
        {
            using var activity = Activity.Current;

            var traceHeaders = new MessageHeaders();
            
            if (activity is not null)
                Propagator.Inject(new PropagationContext(activity.Context, Baggage.Current), traceHeaders,
                    (dict, k, v) => dict[k] = Encoding.UTF8.GetBytes(v));

            var producer = producerAccessor.GetProducer(producerName);
            await producer.ProduceAsync(key, payload, headers: traceHeaders);

            activity?.SetTag("messaging.system", "kafka");
            activity?.SetTag("messaging.operation", "publish");
            activity?.SetTag("messaging.destination", producerName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Something bad happens during messaging producing.");
            throw new ArgumentException($"Something bad happens during message producing on {nameof(producerName)}");
        }
    }
}