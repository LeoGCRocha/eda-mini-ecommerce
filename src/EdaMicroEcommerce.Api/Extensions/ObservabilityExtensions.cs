using EdaMicroEcommerce.Application.Outbox;
using Npgsql;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;

namespace EdaMicroEcommerce.Api.Extensions;

public static class ObservabilityExtensions
{
    public static IServiceCollection AddTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
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
                    .AddSource("CatalogSource")
                    .AddSource("OrdersSource")
                    .AddSource("BillingSource");

                tracing.AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(endpointExporter);
                });
            });
        
        return services;
    }
}