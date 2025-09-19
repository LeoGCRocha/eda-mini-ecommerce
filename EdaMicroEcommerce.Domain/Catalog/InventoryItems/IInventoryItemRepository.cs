namespace EdaMicroEcommerce.Domain.Catalog.InventoryItems;

public interface IInventoryItemRepository
{
    Task AddInventoryItemAsync(InventoryItem inventoryItem);
}