using Carter;
using EdaMicroEcommerce.Api.Extensions;
using EdaMicroEcommerce.Api.OutboxWorker;
using EdaMicroEcommerce.Application.IntegrationEvents;
using EdaMicroEcommerce.Infra;
using EdaMicroEcommerce.Infra.Configuration;
using EdaMicroEcommerce.Infra.MessageBroker;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MessageBrokerConfiguration>(
    builder.Configuration.GetSection("MessageBroker"));

var services = builder.Services;

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddCarter();

var appConfiguration = builder.Configuration;

// TODO: Add schema registry
services.AddTransient<IIntegrationEventPublisher, IntegrationEventPublisher>();

// <TIP> kafka-topics --delete --topic product-deactivated --bootstrap-server localhost:9092
var messageBrokerSection = builder.Configuration.GetSection("MessageBroker");
var messageBroker = messageBrokerSection.Get<MessageBrokerConfiguration>();
if (messageBroker is null)
    throw new Exception("O Message Broker precisa estar definido corretamente.");

services.AddDatabase(appConfiguration)
    .AddMediator()
    .AddProductInventoryServices()
    .AddKafka(messageBroker);

services.AddHostedService<OutboxWorker>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapCarter();
app.Run();