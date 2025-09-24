using KafkaFlow.Producers;
using EdaMicroEcommerce.Application.Outbox;
using EdaMicroEcommerce.Application.IntegrationEvents;
using EdaMicroEcommerce.Infra.MessageBroker.ProducerBuilder;
using EdaMicroEcommerce.Application.IntegrationEvents.Products;
using EdaMicroEcommerce.Infra.MessageBroker.Builders;
using EdaMicroEcommerce.Infra.MessageBroker.Products;

namespace EdaMicroEcommerce.Infra.MessageBroker;

public class IntegrationEventPublisher : IIntegrationEventPublisher
{
    private readonly IProducerAccessor _producerAccessor;

    private static readonly Dictionary<string, ProducerBase> Producers = new()
    {
        { nameof(ProductDeactivationIntegration), new ProductDeactivated() }
    };

    public IntegrationEventPublisher(IProducerAccessor producerAccessor)
    {
        _producerAccessor = producerAccessor;
    }

    public async Task PublishOnTopicAsync<T>(T @event) where T : OutboxIntegrationEvent
    {
        if (!Producers.TryGetValue(@event.Type, out ProducerBase? producerBase))
            throw new Exception("Unexpected type was passed.");
        
        var producer = _producerAccessor.GetProducer(producerBase.Name);
        await producer.ProduceAsync(null, @event.Payload); // When null is passed Kafka uses round-robin
    }
}