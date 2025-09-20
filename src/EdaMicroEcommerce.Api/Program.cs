using Carter;
using EdaMicroEcommerce.Api.Extensions;
using EdaMicroEcommerce.Api.OutboxWorker;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCarter();

var appConfiguration = builder.Configuration;

builder.Services
    .AddServices()
    .AddMediator()
    .AddInfra(appConfiguration);

builder.Services.AddHostedService<OutboxWorker>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapCarter();
app.Run();
