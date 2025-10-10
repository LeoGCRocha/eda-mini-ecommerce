using KafkaFlow.Producers;
using Catalog.Application.IntegrationEvents;

namespace EdaMicroEcommerce.Infra.MessageBroker;

public class IntegrationEventPublisher(IProducerAccessor producerAccessor) : IIntegrationEventPublisher
{
    public async Task PublishOnTopicAsync<T>(T payload, string producerName, string? key = null)
    {
        try
        {
            var producer = producerAccessor.GetProducer(producerName);
            await producer.ProduceAsync(key, payload); // When null is passed Kafka uses round-robin
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Não foi encontrada uma implementação para o produtor informado.");
        }
    }
}