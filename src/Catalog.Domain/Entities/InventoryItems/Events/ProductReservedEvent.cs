using EdaMicroEcommerce.Domain.BuildingBlocks;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;
using EdaMicroEcommerce.Domain.Enums;

namespace Catalog.Domain.Entities.InventoryItems.Events;

public class ProductReservedEvent : IDomainEvent
{
    public ProductId ProductId { get; set; }
    public InventoryItemId InventoryItemId { get; set; }
    public int ReservedQuantity { get; set; }
    public OrderId OrderId { get; set; }
    public ReservationEventType ReservationEventType { get; set; }

    public ProductReservedEvent(ProductId productId, InventoryItemId inventoryItemId, int reservedQuantity, OrderId orderId, ReservationEventType reservationEventType)
    {
        ProductId = productId;
        InventoryItemId = inventoryItemId;
        ReservedQuantity = reservedQuantity;
        OrderId = orderId;
        ReservationEventType = reservationEventType;
    }
}