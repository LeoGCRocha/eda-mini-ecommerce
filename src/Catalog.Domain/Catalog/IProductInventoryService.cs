using Catalog.Domain.Catalog.Products;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Catalog.Domain.Catalog;

public interface IProductInventoryService
{
    Task CreateProductAndInventoryAsync(Product product, int availableQuantity, int reorderQuantity);
    Task DeactivateProductAsync(ProductId productId);
    Task DeactivateProductOnInventoryAsync(ProductId productId);
}