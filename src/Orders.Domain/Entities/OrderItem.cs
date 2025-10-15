using Orders.Domain.Entities.Exceptions;
using EdaMicroEcommerce.Domain.BuildingBlocks;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Orders.Domain.Entities;

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
            throw new InvalidQuantityException("Quantidade precisa maior que zero.");
    }

    public decimal Total()
    {
        return Quantity * UnitPrice;
    }
}