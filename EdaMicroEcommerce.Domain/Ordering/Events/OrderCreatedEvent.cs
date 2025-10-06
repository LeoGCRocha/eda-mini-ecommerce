using EdaMicroEcommerce.Domain.BuildingBlocks;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace EdaMicroEcommerce.Domain.Ordering.Events;

public class OrderCreatedEvent : IDomainEvent
{
    public OrderId OrderId { get; set; }
    public List<ProductOrderInfo> ProductOrderInfos;

    public OrderCreatedEvent(OrderId orderId, List<ProductOrderInfo> productOrderInfos)
    {
        OrderId = orderId;
        ProductOrderInfos = productOrderInfos;
    }
}

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