using EdaMicroEcommerce.Domain.BuildingBlocks;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace EdaMicroEcommerce.Domain.Ordering;

public class OrderItem : Entity<OrderItemId>
{
    public int Quantity { get; private set; }
    public ProductId ProductId { get; private set; }
    public decimal UnitPrice { get; private set; }

    public OrderItem(ProductId productId, int quantity, decimal unitPrice)
    {
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;

        if (Quantity <= 0)
            // TODO: Domain exception
            throw new Exception("");
    }

    public decimal Total()
    {
        return Quantity * UnitPrice;
    }
}