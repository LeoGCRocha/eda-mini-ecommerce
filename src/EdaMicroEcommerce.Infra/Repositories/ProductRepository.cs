using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;
using EdaMicroEcommerce.Domain.Catalog.Products;
using EdaMicroEcommerce.Infra.Persistence;
using Microsoft.EntityFrameworkCore;

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

    public async Task<Product?> GetProductAsync(ProductId productId)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
        return product;
    }

    public Task GetProductAsync(List<ProductId> productIds)
    {
        throw new NotImplementedException();
    }
}