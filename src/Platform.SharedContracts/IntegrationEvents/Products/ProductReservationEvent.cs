using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;
using EdaMicroEcommerce.Domain.Enums;

namespace Platform.SharedContracts.IntegrationEvents.Products;

public class ProductReservationEvent(
    OrderId orderId,
    ProductId productId,
    int quantity,
    ReservationEventType reservationType)
{
    public OrderId OrderId { get; set; } = orderId;
    public ProductId ProductId { get; set; } = productId;
    public int Quantity { get; set; } = quantity;
    public ReservationEventType ReservationType { get; set; } = reservationType;
}