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
        // <WARNING....>
        // The SagaOrchestrator could handle this.
        try
        {
            await productInventoryService.DeactivateProductOnInventoryAsync(message.ProductId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Something bad happens during messaging consumption.");
            throw;
        }
    }
}