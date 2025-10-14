using Catalog.Domain.Entities.Products;
using Microsoft.EntityFrameworkCore;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Catalog.Infra.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly CatalogContext _context;

    public ProductRepository(CatalogContext context)
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