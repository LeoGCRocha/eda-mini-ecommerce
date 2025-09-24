using KafkaFlow;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EdaMicroEcommerce.Infra.MessageBroker.Products;
using EdaMicroEcommerce.Infra.MessageBroker.Builders;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

var kafkaBootstrap = configuration.GetConnectionString("KafkaBootstrapServer");
if (kafkaBootstrap is null)
    throw new ArgumentException("Kafka bootstrap server shoul be defined.");

var cluster = new ClusterBuilder()
    .WithBrokers(kafkaBootstrap)
    .WihConsumer(new ProductDeactivatedConsumer());

var services = new ServiceCollection();
services.AddLogging(configure => configure.AddConsole());

services.AddKafkaFlowHostedService(kafka => 
    cluster.CreateBuilder(kafka));