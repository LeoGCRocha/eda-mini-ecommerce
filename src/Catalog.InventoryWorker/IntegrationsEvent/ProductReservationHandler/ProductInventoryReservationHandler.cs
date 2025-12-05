using KafkaFlow;
using System.Diagnostics;
using Catalog.Domain.Entities;
using Microsoft.Extensions.Logging;
using EdaMicroEcommerce.Domain.Enums;
using Platform.SharedContracts.IntegrationEvents.Products;

namespace EcaMicroEcommerce.ProductWorker.IntegrationsEvent.ProductReservationHandler;

public class ProductInventoryReservationHandler(
    IProductInventoryService productInventoryService,
    ILogger<ProductInventoryReservationHandler> logger)
    : IMessageHandler<ProductReservationEvent>
{
    public async Task Handle(IMessageContext context, ProductReservationEvent message)
    {
        var activity = Activity.Current;

        activity?.SetTag("order.id", message.OrderId.Value.ToString());
        activity?.SetTag("product.id", message.ProductId.Value.ToString());
        activity?.SetTag("operation.type", nameof(message.ReservationType));
        
        // <WARNING> Aqui não estamos lidando indepo, apesar do SAGA não enviar duplicado, se por algum motivo/intermitência ou má implementação
        // ter um segundo envio aqui teriamos um problema
        if (message.ReservationType == ReservationEventType.Reservation)
        {
            activity?.AddEvent(new ActivityEvent("inventory.service.call", 
                tags: new ActivityTagsCollection { { "operation", "ReserveProductIfAvailable" } }));
            
            logger.LogWarning("Realizando reserva do produto {Product}", message.ProductId.Value);
        
            var successfulReserved = await productInventoryService.ReserveProductIfAvailable(message.ProductId, message.Quantity, message.OrderId);

            activity?.AddTag("reservation.successful", successfulReserved);
            
            logger.LogWarning(
                successfulReserved
                    ? "Reserva do produto {Product}, realizada com sucesso"
                    : "Não foi possível realizar a reserva do produto {ProductId}", message.ProductId.Value);
            
            return;
        }

        // <WARNING> Aqui temos o mesmo problema de indepotencia, apesar de controlado no SAGA não é controlado aqui podendo gerar uma incoerência
        if (message.ReservationType == ReservationEventType.Cancellation)
        {
            logger.LogWarning("Canceling product reservation {Product}", message.ProductId.Value);

            await productInventoryService.CancelProductReservation(message.OrderId, message.ProductId, message.Quantity);
            
            return;
        }

        if (message.ReservationType == ReservationEventType.Confirmation)
        {
            logger.LogWarning("Confirming product reservation {Product}", message.ProductId.Value);

            await productInventoryService.ConfirmProductReservation(message.OrderId, message.ProductId,
                message.Quantity);
        }
    }
}