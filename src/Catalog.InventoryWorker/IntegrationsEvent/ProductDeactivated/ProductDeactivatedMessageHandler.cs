using EdaMicroEcommerce.Domain.Catalog;
using KafkaFlow;
using Microsoft.Extensions.Logging;
using EdaMicroEcommerce.Domain.Catalog.Products.Events;

namespace EcaMicroEcommerce.ProductWorker.IntegrationsEvent.ProductDeactivated;

public class ProductDeactivatedMessageHandler : IMessageHandler<ProductDeactivatedEvent>
{
    private readonly ILogger<ProductDeactivatedMessageHandler> _logger;
    private readonly IProductInventoryService _productInventoryService;

    public ProductDeactivatedMessageHandler(ILogger<ProductDeactivatedMessageHandler> logger, IProductInventoryService productInventoryService)
    {
        _logger = logger;
        _productInventoryService = productInventoryService;
    }

    public async Task Handle(IMessageContext context, ProductDeactivatedEvent message)
    {
        _logger.LogInformation("Iniciando processamento da mensagem {0}", context.ConsumerContext.Offset);
        await _productInventoryService.DeactivateProductOnInventoryAsync(message.ProductId);
        _logger.LogInformation("Processamento finalizado, mensagem commitada.");
    }
}