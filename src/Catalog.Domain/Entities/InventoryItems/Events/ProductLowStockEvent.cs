using EdaMicroEcommerce.Domain.BuildingBlocks;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Catalog.Domain.Entities.InventoryItems.Events;

public class ProductLowStockEvent(ProductId productId, InventoryItemId inventoryItemId, int reorderThreshold)
    : IDomainEvent
{
    public ProductId ProductId { get; set; } = productId;
    public InventoryItemId InventoryItemId { get; set; } = inventoryItemId;
    public int ReorderThreshold { get; set; } = reorderThreshold;
}