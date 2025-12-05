using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Billing.PaymentWorker.Extensions;

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
        
        services
            .AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName)) 
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddNpgsql()
                    .AddSource("BillingSource");

                tracing.AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(endpointExporter);
                });
            });
        
        return services;
    }
}