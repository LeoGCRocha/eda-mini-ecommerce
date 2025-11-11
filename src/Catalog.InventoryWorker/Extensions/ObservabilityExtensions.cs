using EdaMicroEcommerce.Infra.MessageBroker;
using Npgsql;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EcaMicroEcommerce.ProductWorker.Extensions;

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
                    .AddSource("CatalogSource");

                tracing.AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(endpointExporter);
                });
            });
        
        return services;
    }
}