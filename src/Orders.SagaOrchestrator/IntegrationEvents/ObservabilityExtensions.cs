using EdaMicroEcommerce.Application.Outbox;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Orders.Saga.IntegrationEvents;

public static class ObservabilityExtensions
{
    public static IServiceCollection AddTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        // A prática utilizada em arquitetura de eventos para controlar o comportamento em todos os serviços se chama
        var serviceName = configuration.GetValue<string>("OTEL_SERVICE_NAME");
        if (serviceName is null)
            throw new ArgumentNullException(nameof(serviceName));
        
        var endpointExporter = configuration.GetValue<string>("OTEL_EXPORTER_OTLP_ENDPOINT") ?? null;
        if (endpointExporter is null)
            throw new ArgumentNullException(endpointExporter);
        
        // ContextPropagation
        services
            .AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName)) // TODO: Using environment
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddNpgsql()
                    .AddSource("OrdersSource");

                tracing.AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(endpointExporter);
                });
            });
        
        return services;
    }
}