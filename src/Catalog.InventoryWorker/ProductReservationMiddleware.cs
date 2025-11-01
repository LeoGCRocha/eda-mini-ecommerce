using KafkaFlow;
using System.Text.Json;
using Catalog.Domain.Entities;
using Microsoft.Extensions.Logging;
using EcaMicroEcommerce.ProductWorker.IntegrationsEvent.ProductReservationHandler;
using Platform.SharedContracts.IntegrationEvents.Products;

namespace EcaMicroEcommerce.ProductWorker;

public class ProductReservationMiddleware : IMessageMiddleware
{
    // <WARNING> Essa forma não é recomendavel porém por algum motivo a forma automatica no KAFKA FLOW não tava resolvendo corretamente.
    public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
        var sp = context.DependencyResolver;
        var bytes = context.Message.Value as byte[];
        var payloadString = bytes is null ? "" : System.Text.Encoding.UTF8.GetString(bytes);

        var message = JsonSerializer.Deserialize<ProductReservationEvent>(payloadString,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (message is not null)
        {
            var handler = new ProductInventoryReservationHandler(sp.Resolve<IProductInventoryService>(),
                sp.Resolve<ILogger<ProductInventoryReservationHandler>>());
            await handler.Handle(context, message);
        }
    }
}