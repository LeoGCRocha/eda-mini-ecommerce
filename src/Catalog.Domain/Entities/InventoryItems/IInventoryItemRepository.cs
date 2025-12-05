using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Catalog.Domain.Entities.InventoryItems;

public interface IInventoryItemRepository
{
    Task AddInventoryItemAsync(InventoryItem inventoryItem);
    Task<InventoryItem?> GetInventoryItemByProductId(ProductId productId);
    
}