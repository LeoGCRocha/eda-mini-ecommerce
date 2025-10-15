using Catalog.Contracts.DTOs;

namespace Catalog.Contracts.Public;

public interface ICatalogApi
{
    public Task<List<ProductAvailabilityResponse>> HasProductsAvailable(List<ProductAvailabilityRequest> request);
}