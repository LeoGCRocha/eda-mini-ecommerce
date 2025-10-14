using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Orders.Domain.Entities.Events;

public class ProductOrderInfo
{
    public ProductId ProductId { get; set; }
    public int Quantity { get; set; }

    public ProductOrderInfo(ProductId productId, int quantity)
    {
        ProductId = productId;
        Quantity = quantity;
    }
}