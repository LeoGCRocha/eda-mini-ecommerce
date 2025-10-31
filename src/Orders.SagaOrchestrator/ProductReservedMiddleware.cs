using System.Text.Json;
using System.Text.Json.Serialization;
using KafkaFlow;
using Orders.Application.Saga;
using Orders.Domain.Entities.Events;
using Orders.Saga.IntegrationEvents;

namespace Orders.Saga;

public class ProductReservedMiddleware : IMessageMiddleware
{
    // <WARNING> Essa forma não é recomendavel, porém por algum motivo a forma automatica no KAFKA FLOW não tava resolvendo corretamente.
    public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
        var sp = context.DependencyResolver;
        var bytes = context.Message.Value as byte[];
        var payloadString = bytes is null ? "" : System.Text.Encoding.UTF8.GetString(bytes);
        
        var message = JsonSerializer.Deserialize<ProductReservationStatusEvent>(payloadString,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter() }});

        if (message is not null)
        {
            var handler = new ProductReservedMessageHandler(sp.Resolve<ISagaOrchestrator>());
            await handler.Handle(context, message);
        }
    }
}