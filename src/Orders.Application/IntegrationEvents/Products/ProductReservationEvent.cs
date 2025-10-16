using MediatR;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Orders.Application.IntegrationEvents.Products;

// TODO: Isso aqui deveria estar num SCHEMA REGISTRY pra não repetir isso em ambos modulos
public class ProductReservationEvent : IRequest
{
    public OrderId OrderId { get; set; }
    public ProductId ProductId { get; set; }
    public int Quantity { get; set; }

    public ProductReservationEvent(OrderId orderId, ProductId productId, int quantity)
    {
        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
    }
}