using EdaMicroEcommerce.Domain.Enums;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Platform.SharedContracts.IntegrationEvents.Products;

public class ProductReservationEvent(
    OrderId orderId,
    ProductId productId,
    int quantity,
    ReservationEventType reservationType)
{
    public OrderId OrderId { get; init; } = orderId;
    public ProductId ProductId { get; init; } = productId;
    public int Quantity { get; init; } = quantity;
    public ReservationEventType ReservationType { get; init; } = reservationType;
}