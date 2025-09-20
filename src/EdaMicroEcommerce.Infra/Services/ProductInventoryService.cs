using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;
using EdaMicroEcommerce.Domain.Catalog;
using EdaMicroEcommerce.Domain.Catalog.InventoryItems;
using EdaMicroEcommerce.Domain.Catalog.Products;
using EdaMicroEcommerce.Infra.Persistence;

namespace EdaMicroEcommerce.Infra.Services;

public class ProductInventoryService : IProductInventoryService
{
    private readonly IProductRepository _productRepository;
    private readonly IInventoryItemRepository _inventoryItemRepository;
    private readonly EdaContext _context;

    public ProductInventoryService(IProductRepository productRepository,
        IInventoryItemRepository inventoryItemRepository, EdaContext context)
    {
        _productRepository = productRepository;
        _inventoryItemRepository = inventoryItemRepository;
        _context = context;
    }

    public async Task CreateProductAndInventoryAsync(Product product, int availableQuantity, int reorderQuantity)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            await _productRepository.AddProductAsync(product);
            var inventory = new InventoryItem(product.Id, availableQuantity, reorderQuantity);
            await _inventoryItemRepository.AddInventoryItemAsync(inventory);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task DeactivateProduct(ProductId productId)
    {
        var product = await _productRepository.GetProductAsync(productId);
        if (product is null)
            // This should be a specific exception to be handled at middleware
            throw new Exception("Product not found.");
        product.DeactivateProduct();
        
        await _context.SaveChangesAsync();
    }
}