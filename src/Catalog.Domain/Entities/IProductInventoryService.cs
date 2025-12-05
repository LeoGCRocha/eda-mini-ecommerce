using Catalog.Contracts.DTOs;
using Catalog.Domain.Entities.Products;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Catalog.Domain.Entities;

public interface IProductInventoryService
{
    Task CreateProductAndInventoryAsync(Product product, int availableQuantity, int reorderQuantity);
    Task DeactivateProductAsync(ProductId productId);
    Task DeactivateProductOnInventoryAsync(ProductId productId);
    Task<List<ProductAvailabilityResponse>> HasAvailabilityForProduct(Dictionary<ProductId, int> productsWithQuantity);
    Task<bool> ReserveProductIfAvailable(ProductId productId, int quantity, OrderId orderId);
    Task CancelProductReservation(OrderId orderId, ProductId productId, int quantity);
    Task ConfirmProductReservation(OrderId orderId, ProductId productId, int quantity);
}