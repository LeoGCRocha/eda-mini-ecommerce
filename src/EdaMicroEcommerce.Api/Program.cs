using Carter;
using KafkaFlow;
using EdaMicroEcommerce.Api.Extensions;
using EdaMicroEcommerce.Api.OutboxWorker;
using EdaMicroEcommerce.Application.IntegrationEvents;
using EdaMicroEcommerce.Infra.MessageBroker;
using EdaMicroEcommerce.Infra.MessageBroker.Builders;
using EdaMicroEcommerce.Infra.MessageBroker.ProducerBuilder;
using EdaMicroEcommerce.Infra.MessageBroker.Products;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCarter();

var appConfiguration = builder.Configuration;

// TODO: Move this to infra
// TODO: Add schema registry
// TODO: Add topics names to appSettings
var kafkaBootstrapServer = appConfiguration.GetConnectionString("KafkaBootstrapServer");
if (kafkaBootstrapServer is null)
    throw new ArgumentException("Should have defined bootstrap server.");

builder.Services.AddScoped<IIntegrationEventPublisher, IntegrationEventPublisher>();
var cluster = new ClusterBuilder()
    .WithBrokers(kafkaBootstrapServer)
    .WithTopic(ProductDeactivated.TopicName, 1, 1)
    .WithProducer(new ProductDeactivated());

builder.Services
    .AddServices()
    .AddMediator()
    .AddInfra(appConfiguration);

builder.Services.AddKafka(kafka => 
     cluster.CreateBuilder(kafka)
);

builder.Services.AddHostedService<OutboxWorker>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapCarter();
app.Run();