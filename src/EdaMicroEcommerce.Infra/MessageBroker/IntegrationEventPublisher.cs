using KafkaFlow;
using KafkaFlow.Producers;
using EdaMicroEcommerce.Application.Outbox;
using EdaMicroEcommerce.Application.IntegrationEvents;

namespace EdaMicroEcommerce.Infra.MessageBroker;

public class IntegrationEventPublisher : IIntegrationEventPublisher
{
    private readonly IProducerAccessor _producerAccessor;
    
    public IntegrationEventPublisher(IProducerAccessor producerAccessor)
    {
        _producerAccessor = producerAccessor;
    }

    public async Task PublishOnTopicAsync<T>(T payload, string producerName, string? key = null)
    {
        try
        {
            var producer = _producerAccessor.GetProducer(producerName);
            await producer.ProduceAsync(key, payload); // When null is passed Kafka uses round-robin
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Não foi encontrada uma implementação para o produtor informado.");
        }
    }
}