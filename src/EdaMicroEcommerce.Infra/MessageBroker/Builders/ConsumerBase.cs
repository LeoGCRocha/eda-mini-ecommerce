using KafkaFlow;
using KafkaFlow.Configuration;
using KafkaFlow.Serializer;

namespace EdaMicroEcommerce.Infra.MessageBroker.Builders;

public abstract class ConsumerBase
{
    private readonly string Name;
    private readonly string _topicName;

    protected ConsumerBase(string topicName)
    {
        Name = GetType().Name;
        _topicName = topicName;
    }

    public Action<IConsumerConfigurationBuilder> CreateConsumerFromBase(int numberOfThreads = 3)
    {
        return consumer =>
        {
            consumer
                .Topic(_topicName)
                .WithGroupId(Name)
                .WithWorkersCount(Math.Max(numberOfThreads, 10))
                .WithBufferSize(100) // maximum messages on queue
                .WithAutoOffsetReset(AutoOffsetReset.Earliest)
                .AddMiddlewares(middlewares =>
                {
                    middlewares.AddDeserializer<JsonCoreDeserializer>();
                    // TODO: Add type handler
                });
        };
    }
}