using System.Text.Json;
using System.Text.Json.Serialization;
using KafkaFlow;
using Orders.Application.Saga;
using Orders.Saga.IntegrationEvents;
using Platform.SharedContracts.IntegrationEvents.Payments;

namespace Orders.Saga.MessageMiddlewares;

public class PaymentProcessedMiddleware : IMessageMiddleware
{
    public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
        var sp = context.DependencyResolver;
        var bytes = context.Message.Value as byte[];
        var payloadString = bytes is null ? "" : System.Text.Encoding.UTF8.GetString(bytes);
        
        var message = JsonSerializer.Deserialize<PaymentProcessedEvent>(payloadString,
            new JsonSerializerOptions
                { PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter() } });

        if (message is not null)
        {
            var handler = new PaymentProcessedMessageHandler(sp.Resolve<ISagaOrchestrator>());
            await handler.Handle(context, message);
        }
    }
}