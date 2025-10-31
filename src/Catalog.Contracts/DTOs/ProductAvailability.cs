namespace Catalog.Contracts.DTOs;

public class ProductAvailabilityRequest(Guid productId, int quantity)
{
    public Guid ProductId { get; set; } = productId;
    public int Quantity { get; set; } = quantity;
}

public class ProductAvailabilityResponse
{
    public Guid ProductId { get; init; }
    public bool AvailableForQuantity { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }

    public ProductAvailabilityResponse(Guid productId, bool availableForQuantity, int quantity, decimal price)
    {
        ProductId = productId;
        AvailableForQuantity = availableForQuantity;
        Quantity = quantity;
        UnitPrice = price;
    }
}