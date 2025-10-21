using MediatR;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;
using EcaMicroEcommerce.ProductWorker.IntegrationsEvent.ProductReservationHandler;

// TODO: Isso aqui deveria estar num SCHEMA REGISTRY pra não repetir isso em ambos modulos
public class ProductReservationEvent : IRequest
{
    public OrderId OrderId { get; set; }
    public ProductId ProductId { get; set; }
    public int Quantity { get; set; }
    public ReservationType ReservationType { get; set; }

    public ProductReservationEvent(OrderId orderId, ProductId productId, int quantity, ReservationType reservationType)
    {
        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
        ReservationType = reservationType;
    }
}