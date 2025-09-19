using EdaMicroEcommerce.Domain.BuildingBlocks;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace EdaMicroEcommerce.Domain.Catalog.InventoryItems.Events;

public class ProductLowStockEvent : IDomainEvent
{
    public ProductId ProductId { get; set; }
    public InventoryItemId InventoryItemId { get; set; }
    public int ReorderThreshold { get; set; }

    public ProductLowStockEvent(ProductId productId, InventoryItemId inventoryItemId, int reorderThreshold)
    {
        ProductId = productId;
        InventoryItemId = inventoryItemId;
        ReorderThreshold = reorderThreshold;
    }
}