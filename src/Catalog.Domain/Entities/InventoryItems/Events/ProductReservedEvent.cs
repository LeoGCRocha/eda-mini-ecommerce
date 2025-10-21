using EdaMicroEcommerce.Domain.BuildingBlocks;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Catalog.Domain.Entities.InventoryItems.Events;

public class ProductReservedEvent : IDomainEvent
{
    public ProductId ProductId { get; set; }
    public InventoryItemId InventoryItemId { get; set; }
    public int ReservedQuantity { get; set; }
    public OrderId OrderId { get; set; }
    public bool IsReservationSucceed { get; set; }

    public ProductReservedEvent(ProductId productId, InventoryItemId inventoryItemId, int reservedQuantity, OrderId orderId, bool isReservationSucceed)
    {
        ProductId = productId;
        InventoryItemId = inventoryItemId;
        ReservedQuantity = reservedQuantity;
        OrderId = orderId;
        IsReservationSucceed = isReservationSucceed;
    }
}