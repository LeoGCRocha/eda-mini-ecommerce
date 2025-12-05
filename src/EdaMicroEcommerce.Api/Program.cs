using Billing.Infras.Outbox;
using Carter;
using Catalog.Infra.Outbox;
using EdaMicroEcommerce.Infra.Configuration;
using EdaMicroEcommerce.Api.Extensions;
using EdaMicroEcommerce.Application.Outbox;
using EdaMicroEcommerce.Domain.BuildingBlocks;
using EdaMicroEcommerce.Infra.MessageBroker;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Orders.Infra.Outbox;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MessageBrokerConfiguration>(
    builder.Configuration.GetSection("MessageBroker"));

var services = builder.Services;

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddCarter();

var appConfiguration = builder.Configuration;

// TODO: Add schema registry
services.AddScoped<IIntegrationEventPublisher, IntegrationEventPublisher>();

// <TIP> kafka-topics --delete --topic product-deactivated --bootstrap-server localhost:9092
var messageBrokerSection = builder.Configuration.GetSection("MessageBroker");
var messageBroker = messageBrokerSection.Get<MessageBrokerConfiguration>();
if (messageBroker is null)
    throw new Exception("O Message Broker precisa estar definido corretamente.");

services
    .AddModulesServices(appConfiguration);

services.AddHostedService<BillingOutboxWorker>();
services.AddHostedService<CatalogOutboxWorker>();
services.AddHostedService<OrderOutboxWorker>();
services.AddTelemetry(appConfiguration);

var app = builder.Build();

app.UseExceptionHandler(exceptHandler =>
{
    exceptHandler.Run(async context =>
    {
        context.Response.ContentType = "application/problem+json";
        
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

        if (exceptionHandlerPathFeature?.Error is Exception ex)
        {
            ProblemDetails problemDetails = new ProblemDetails()
            {
                Type = ex.GetType().Name,
                Status = StatusCodes.Status500InternalServerError,
                Title = "Ocorreu um erro inesperado",
                Detail = ex.Message,
                Instance = exceptionHandlerPathFeature.Path
            };

            if (ex is DomainException or GenericException)
            {
                problemDetails.Type = ex.GetType().Name;
                problemDetails.Status = StatusCodes.Status500InternalServerError;
            }

            context.Response.StatusCode = problemDetails.Status.GetValueOrDefault();
            
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapCarter();
app.Run();