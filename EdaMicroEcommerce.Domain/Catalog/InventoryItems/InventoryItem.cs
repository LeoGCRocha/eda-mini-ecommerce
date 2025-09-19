using EdaMicroEcommerce.Domain.BuildingBlocks;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;
using EdaMicroEcommerce.Domain.Catalog.InventoryItems.Events;
using EdaMicroEcommerce.Domain.Catalog.InventoryItems.Exceptions;

namespace EdaMicroEcommerce.Domain.Catalog.InventoryItems;

public sealed class InventoryItem : AggregateRoot<InventoryItemId>
{
    public ProductId ProductId { get; private set; }
    public int AvailableQuantity { get; private set; }
    public int ReservedQuantity { get; private set; }
    public int ReorderLevel { get; private set; }

    private InventoryItem() {} // Ef
    
    public InventoryItem(ProductId productId, int availableQuantity = 0, int reorderLevel = 0, int reservedQuantity = 0)
    {
        ProductId = productId;
        AvailableQuantity = availableQuantity;
        ReservedQuantity = reservedQuantity;
        ReorderLevel = reorderLevel;
    }

    public void ReserveQuantity(int quantity)
    {
        if (quantity > AvailableQuantity)
            throw new InventoryItemInvalidReservationException("Quantity to reserve cannot be greater than the available.");
            
        ReservedQuantity += quantity;
        AvailableQuantity -= quantity;

        AddDomainEvent(new ProductReservedEvent(ProductId, Id, quantity));
        if (AvailableQuantity <= ReorderLevel)
            AddDomainEvent(new ProductLowStockEvent(ProductId, Id, ReorderLevel));
    }
}