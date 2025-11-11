namespace Catalog.Contracts.DTOs;

public class ProductAvailabilityRequest(Guid productId, int quantity)
{
    public Guid ProductId { get; set; } = productId;
    public int Quantity { get; set; } = quantity;
}