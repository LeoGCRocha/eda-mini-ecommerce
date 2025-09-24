using Confluent.Kafka;
using KafkaFlow;
using KafkaFlow.Configuration;
using KafkaFlow.Serializer;

namespace EdaMicroEcommerce.Infra.MessageBroker.Builders;

public abstract class ProducerBase
{
    public readonly string Name;
    private readonly string _topicName;

    protected ProducerBase(string topicName)
    {
        Name = GetType().Name;
        _topicName = topicName;
    }

    public Action<IProducerConfigurationBuilder> CreateProducerFromBase(ProducerConfig? customerConfig = null)
    {
        return producer =>
        {
            producer.DefaultTopic(_topicName)
                .AddMiddlewares(m => m.AddSerializer<JsonCoreSerializer>())
                .WithProducerConfig(customerConfig ??
                                    new ProducerConfig
                                    {
                                        Acks = Confluent.Kafka.Acks.Leader,
                                        EnableIdempotence = false,
                                        LingerMs = 25,
                                        CompressionType = CompressionType.Lz4,
                                        BatchSize = 5_242_880 // 5 Mb
                                    });
        };
    }
}