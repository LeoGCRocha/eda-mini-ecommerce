using EdaMicroEcommerce.Domain.Catalog.InventoryItems;
using EdaMicroEcommerce.Infra.Persistence;

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
}