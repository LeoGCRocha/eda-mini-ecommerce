using EdaMicroEcommerce.Infra.MessageBroker.ProducerBuilder;
using KafkaFlow.Configuration;

namespace EdaMicroEcommerce.Infra.MessageBroker.Builders;

public class ClusterBuilder
{
    private string[] _brokers = [];
    private readonly List<ProducerBase> _producerBases = new();
    private readonly List<ConsumerBase> _consumerBases = new();

    private List<(string topicName, int partitions, short replica)> _topics = new ();
    
    public ClusterBuilder WithBrokers(params string[] brokers)
    {
        if (brokers.Length == 0)
            throw new Exception("At least one broker should be passed.");
        
        _brokers = brokers;
        return this;
    }

    public ClusterBuilder WithTopic(string topicName, int partitions = 1, short replica = 1)
    {
        _topics.Add((topicName, partitions, replica));
        return this;
    }

    public ClusterBuilder WithProducer(ProducerBase producerBase)
    {
        _producerBases.Add(producerBase);
        return this;
    }

    public ClusterBuilder WihConsumer(ConsumerBase consumerBase)
    {
        _consumerBases.Add(consumerBase);
        return this;
    }

    public void CreateBuilder(IKafkaConfigurationBuilder kafka)
    {
        kafka.AddCluster(cluster =>
            {
                cluster.WithBrokers(_brokers);

                foreach (var topic in _topics)
                    cluster.CreateTopicIfNotExists(topic.topicName, topic.partitions, topic.replica);
                    
                foreach (var producer in _producerBases)
                    cluster.AddProducer(producer.Name, producer.CreateProducerFromBase());

                foreach (var consumer in _consumerBases)
                    cluster.AddConsumer(consumer.CreateConsumerFromBase());
            }
        );
    }
}