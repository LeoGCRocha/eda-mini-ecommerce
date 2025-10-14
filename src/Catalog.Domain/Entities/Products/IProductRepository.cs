using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Catalog.Domain.Entities.Products;

public interface IProductRepository
{
    Task AddProductAsync(Product product);
    Task<Product?> GetProductAsync(ProductId productId);
    Task GetProductAsync(List<ProductId> productIds);
}