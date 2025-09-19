using EdaMicroEcommerce.Domain.Catalog.Products;

namespace EdaMicroEcommerce.Domain.Catalog;

public interface IProductInventoryService
{
    Task CreateProductAndInventoryAsync(Product product, int availableQuantity, int reorderQuantity);
}