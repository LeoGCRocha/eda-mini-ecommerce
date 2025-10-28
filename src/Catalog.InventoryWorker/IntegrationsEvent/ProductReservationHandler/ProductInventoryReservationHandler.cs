using KafkaFlow;
using Catalog.Domain.Entities;
using EdaMicroEcommerce.Domain.Enums;
using Microsoft.Extensions.Logging;
using Platform.SharedContracts.IntegrationEvents.Products;

namespace EcaMicroEcommerce.ProductWorker.IntegrationsEvent.ProductReservationHandler;

public class ProductInventoryReservationHandler : IMessageHandler<ProductReservationEvent>
{
    private readonly IProductInventoryService _productInventoryService;
    private readonly ILogger<ProductInventoryReservationHandler> _logger;

    public ProductInventoryReservationHandler(IProductInventoryService productInventoryService, ILogger<ProductInventoryReservationHandler> logger)
    {
        _productInventoryService = productInventoryService;
        _logger = logger;
    }

    public async Task Handle(IMessageContext context, ProductReservationEvent message)
    {
        // <WARNING> Aqui não estamos lidando indepo, apesar do SAGA não enviar duplicado, se por algum motivo/intermitência ou má implementação
        // ter um segundo envio aqui teriamos um problema
        if (message.ReservationType == ReservationEventType.Reservation)
        {
            _logger.LogWarning("Realizando reserva do produto {Product}", message.ProductId.Value);
        
            var successfulReserved = await _productInventoryService.ReserveProductIfAvailable(message.ProductId, message.Quantity, message.OrderId);
        
            _logger.LogWarning(
                successfulReserved
                    ? "Reserva do produto {Product}, realizada com sucesso"
                    : "Não foi possível realizar a reserva do produto {ProductId}", message.ProductId.Value);
            
            return;
        }

        // <WARNING> Aqui temos o mesmo problema de indepotencia, apesar de controlado no SAGA não é controlado aqui podendo gerar uma incoerência
        if (message.ReservationType == ReservationEventType.Cancellation)
        {
            _logger.LogWarning("Cancelando a reserva do produto {Product}", message.ProductId.Value);

            await _productInventoryService.CancelProductReservation(message.OrderId, message.ProductId, message.Quantity);
            
            return;
        }

        if (message.ReservationType == ReservationEventType.Confirmation)
        {
            // TODO: Aqui ao finalizar o PAGAMENTO SE CONCLUIR DAI TIRA A QUANTIDADE RESERVADA E TIRA QUANTIDADE AVAILABLE POR QUE JÁ FOI CONCRETIZADO
        }
    }
}