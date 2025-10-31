using Catalog.Contracts.DTOs;
using Catalog.Contracts.Public;
using Catalog.Domain.Entities;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Catalog.Application;

public class CatalogApi : ICatalogApi
{
    private readonly IProductInventoryService _productInventoryService;

    public CatalogApi(IProductInventoryService productInventoryService)
    {
        _productInventoryService = productInventoryService;
    }

    public async Task<List<ProductAvailabilityResponse>> HasProductsAvailable(List<ProductAvailabilityRequest> request)
    {
        Dictionary<ProductId, int> inputDesired = request.ToDictionary(req => new ProductId(req.ProductId), req => req.Quantity);
        return await _productInventoryService.HasAvailabilityForProduct(inputDesired);
    }
}