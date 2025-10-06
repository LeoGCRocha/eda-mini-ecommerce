using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;
using EdaMicroEcommerce.Domain.Catalog.Products;

namespace EdaMicroEcommerce.Domain.Catalog;

public interface IProductInventoryService
{
    Task CreateProductAndInventoryAsync(Product product, int availableQuantity, int reorderQuantity);
    Task DeactivateProductAsync(ProductId productId);
    Task DeactivateProductOnInventoryAsync(ProductId productId);
}