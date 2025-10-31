using Catalog.Domain.Entities;
using Catalog.Domain.Entities.Products.Events;
using KafkaFlow;
using Microsoft.Extensions.Logging;

namespace EcaMicroEcommerce.ProductWorker.IntegrationsEvent.ProductDeactivated;

public class ProductDeactivatedMessageHandler(
    ILogger<ProductDeactivatedMessageHandler> logger,
    IProductInventoryService productInventoryService)
    : IMessageHandler<ProductDeactivatedEvent>
{
    public async Task Handle(IMessageContext context, ProductDeactivatedEvent message)
    {
        // TODO: Preciso lidar com isso aqui tambem no SAGA....
        logger.LogInformation("Iniciando processamento da mensagem offset {0}", context.ConsumerContext.Offset);
        await productInventoryService.DeactivateProductOnInventoryAsync(message.ProductId);
        logger.LogInformation("Processamento finalizado, mensagem commitada.");
    }
}