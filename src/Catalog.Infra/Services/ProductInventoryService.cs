using Catalog.Domain.Catalog;
using Catalog.Domain.Catalog.Products;
using Catalog.Domain.Catalog.InventoryItems;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Catalog.Infra.Services;

public class ProductInventoryService(
    IProductRepository productRepository,
    IInventoryItemRepository inventoryItemRepository,
    CatalogContext context)
    : IProductInventoryService
{
    public async Task CreateProductAndInventoryAsync(Product product, int availableQuantity, int reorderQuantity)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            await productRepository.AddProductAsync(product);
            var inventory = new InventoryItem(product.Id, availableQuantity, reorderQuantity);
            await inventoryItemRepository.AddInventoryItemAsync(inventory);

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task DeactivateProductAsync(ProductId productId)
    {
        var product = await productRepository.GetProductAsync(productId);
        if (product is null)
            // This should be a specific exception to be handled at middleware
            throw new Exception("Produto inexistente.");
        product.DeactivateProduct();
        
        await context.SaveChangesAsync();
    }

    public async Task DeactivateProductOnInventoryAsync(ProductId productId)
    {
        var inventoryByProduct = await inventoryItemRepository.GetInventoryItemByProductId(productId);
        inventoryByProduct.MakeUnavailable();
        await context.SaveChangesAsync();
    }
}