using Catalog.Domain.Entities.Products;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Catalog.Domain.Entities;

public interface IProductInventoryService
{
    Task CreateProductAndInventoryAsync(Product product, int availableQuantity, int reorderQuantity);
    Task DeactivateProductAsync(ProductId productId);
    Task DeactivateProductOnInventoryAsync(ProductId productId);
}