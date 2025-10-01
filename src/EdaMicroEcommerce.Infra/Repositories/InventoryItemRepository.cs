using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;
using EdaMicroEcommerce.Domain.Catalog.InventoryItems;
using EdaMicroEcommerce.Infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EdaMicroEcommerce.Infra.Repositories;

public class InventoryItemRepository : IInventoryItemRepository
{
    private readonly EdaContext _context;

    public InventoryItemRepository(EdaContext context)
    {
        _context = context;
    }

    public async Task AddInventoryItemAsync(InventoryItem inventoryItem)
    {
        await _context.AddRangeAsync(inventoryItem);
    }

    public async Task<InventoryItem?> GetInventoryItemByProductId(ProductId productId)
    {
        return await _context.InventoryItems.FirstOrDefaultAsync(inventory => inventory.ProductId == productId);
    }
}