using Catalog.Domain.Entities;
using KafkaFlow;
using Microsoft.Extensions.Logging;

namespace EcaMicroEcommerce.ProductWorker.IntegrationsEvent.ProductReservationHandler;

public class ProductReservationHandler : IMessageHandler<ProductReservationEvent>
{
    private readonly IProductInventoryService _productInventoryService;
    private readonly ILogger<ProductReservationHandler> _logger;


    public ProductReservationHandler(IProductInventoryService productInventoryService, ILogger<ProductReservationHandler> logger)
    {
        _productInventoryService = productInventoryService;
        _logger = logger;
    }

    public async Task Handle(IMessageContext context, ProductReservationEvent message)
    {
        _logger.LogWarning("Realizando reserva do produto {Product}", message.ProductId.Value);
        
        var successfulReserved = await _productInventoryService.ReserveProductIfAvailable(message.ProductId, message.Quantity);
        
        _logger.LogWarning(
            successfulReserved
                ? "Reserva do produto {Product}, realizada com sucesso"
                : "Não foi possível realizar a reserva do produto {ProductId}", message.ProductId.Value);
    }
}