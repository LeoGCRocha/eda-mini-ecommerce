using KafkaFlow;
using Catalog.Domain.Catalog;
using Microsoft.Extensions.Logging;
using Catalog.Domain.Catalog.Products.Events;

namespace EcaMicroEcommerce.ProductWorker.IntegrationsEvent.ProductDeactivated;

public class ProductDeactivatedMessageHandler(
    ILogger<ProductDeactivatedMessageHandler> logger,
    IProductInventoryService productInventoryService)
    : IMessageHandler<ProductDeactivatedEvent>
{
    public async Task Handle(IMessageContext context, ProductDeactivatedEvent message)
    {
        logger.LogInformation("Iniciando processamento da mensagem offset {0}", context.ConsumerContext.Offset);
        await productInventoryService.DeactivateProductOnInventoryAsync(message.ProductId);
        logger.LogInformation("Processamento finalizado, mensagem commitada.");
    }
}