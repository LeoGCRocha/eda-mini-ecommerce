using Catalog.Domain.Entities.InventoryItems;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infra.Repositories;

public class InventoryItemRepository(CatalogContext context) : IInventoryItemRepository
{
    public async Task AddInventoryItemAsync(InventoryItem inventoryItem)
    {
        await context.AddRangeAsync(inventoryItem);
    }

    public async Task<InventoryItem?> GetInventoryItemByProductId(ProductId productId)
    {
        return await context.InventoryItems.FirstOrDefaultAsync(inventory => inventory.ProductId == productId);
    }
}