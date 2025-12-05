using KafkaFlow;
using OpenTelemetry;
using System.Diagnostics;
using OpenTelemetry.Context.Propagation;

namespace EdaMicroEcommerce.Application;

public class MessageContextPropagationMiddleware(ActivitySource activitySource) : IMessageMiddleware
{
    private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;
    public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
        var parentContext = Propagator.Extract(default, context.Headers, ContextPropagationHelper.ExtractHeader);
        Baggage.Current = parentContext.Baggage;

        using var activity = activitySource.StartActivity(
            $"KafkaFlow:Processing:{context.Message.GetType().Name}",
            ActivityKind.Consumer,
            parentContext.ActivityContext);
        
        activity?.SetTag("message.system", "kafka");
        activity?.SetTag("messaging.operation", "consuming");
        activity?.SetTag("messaging.operation", "process");

        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            activity?.SetTag("error", true);
            
            throw;
        }
        finally
        {
            activity?.Stop();;
        }
    }
}