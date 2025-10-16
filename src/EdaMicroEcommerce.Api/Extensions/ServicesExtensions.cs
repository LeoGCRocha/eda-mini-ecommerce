
using KafkaFlow;
using Orders.Api;
using Catalog.Api;
using Confluent.Kafka;
using EdaMicroEcommerce.Infra.Configuration;
using KafkaFlow.Serializer;
using Acks = Confluent.Kafka.Acks;

namespace EdaMicroEcommerce.Api.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddModulesServices(this IServiceCollection services, IConfiguration appConfiguration)
    {
        // <TIP> kafka-topics --delete --topic product-deactivated --bootstrap-server localhost:9092
        var messageBrokerSection = appConfiguration.GetSection("MessageBroker");
        var messageBroker = messageBrokerSection.Get<MessageBrokerConfiguration>();
        if (messageBroker is null)
            throw new Exception("O Message Broker precisa estar definido corretamente.");
        
        services
            .AddCatalog(appConfiguration)
            .AddOrders(appConfiguration)
            .AddMessageBroker(messageBroker);
        
        return services;
    }
    
    private static IServiceCollection AddMessageBroker(this IServiceCollection services,
        MessageBrokerConfiguration messageBrokerConfiguration)
    {
        Dictionary<string, ProducerConfiguration> producerConfigurations = new();
        
        CatalogServicesExtensions.RegisterProducers(producerConfigurations, messageBrokerConfiguration);
        OrderServicesExtensions.RegisterProducers(producerConfigurations, messageBrokerConfiguration);
        
        services
            .AddKafka(kafka => 
                kafka.UseConsoleLog()
                    .AddCluster(cluster =>
                        {
                            cluster.WithBrokers([messageBrokerConfiguration.BootstrapServers]);

                            foreach (var producerConfiguration in producerConfigurations.Values)
                            {
                                cluster.CreateTopicIfNotExists(producerConfiguration.Topic,
                                    producerConfiguration.Partitions, producerConfiguration.ReplicaFactor);
                            }

                            foreach (var (name, producerConfiguration) in producerConfigurations)
                            {
                                 // <WARNING> pensar uma maneira de pegar mais configs pra definir mais robustez na produção
                                 // Adicionar mais variaveis que vão possuir valor default.
                                // TODO: Adicionar aqui configs
                                cluster.AddProducer(name, producer =>
                                {
                                    producer.DefaultTopic(producerConfiguration.Topic)
                                        // TODO: Deixar configurações menos genericas
                                        .WithProducerConfig(new ProducerConfig()
                                        {
                                            EnableIdempotence = true, // So garante indepo. em caso de retry, não garante pra se o broker reiniciar
                                                                      // e/ou se tiver falha no outbox
                                            Acks = Acks.Leader,
                                            LingerMs = 5,
                                            BatchNumMessages = 5000,
                                            BatchSize = 32_768,  // 32 KB
                                            CompressionType = CompressionType.Snappy
                                        })
                                        .AddMiddlewares(m => m.AddSerializer<JsonCoreSerializer>());
                                });
                            }
                        }
                    ));
        
        return services;
    }
}