using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Catalog.Domain.Catalog.Products;

public interface IProductRepository
{
    Task AddProductAsync(Product product);
    Task<Product?> GetProductAsync(ProductId productId);
    Task GetProductAsync(List<ProductId> productIds);
}