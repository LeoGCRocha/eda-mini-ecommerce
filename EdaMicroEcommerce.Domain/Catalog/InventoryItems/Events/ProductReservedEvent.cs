using EdaMicroEcommerce.Domain.BuildingBlocks;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace EdaMicroEcommerce.Domain.Catalog.InventoryItems.Events;

public class ProductReservedEvent : IDomainEvent
{
    public ProductId ProductId { get; set; }
    public InventoryItemId InventoryItemId { get; set; }
    public int ReservedQuantity { get; set; }

    public ProductReservedEvent(ProductId productId, InventoryItemId inventoryItemId, int reservedQuantity)
    {
        ProductId = productId;
        InventoryItemId = inventoryItemId;
        ReservedQuantity = reservedQuantity;
    }
}