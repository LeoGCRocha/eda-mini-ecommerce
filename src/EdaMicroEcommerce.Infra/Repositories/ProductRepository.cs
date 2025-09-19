using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;
using EdaMicroEcommerce.Domain.Catalog.Products;
using EdaMicroEcommerce.Infra.Persistence;

namespace EdaMicroEcommerce.Infra.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly EdaContext _context;

    public ProductRepository(EdaContext context)
    {
        _context = context;
    }

    public async Task AddProductAsync(Product product)
    {
        await _context.AddRangeAsync(product);
    }

    public Task GetProductAsync(ProductId productId)
    {
        throw new NotImplementedException();
    }

    public Task GetProductAsync(List<ProductId> productIds)
    {
        throw new NotImplementedException();
    }
}