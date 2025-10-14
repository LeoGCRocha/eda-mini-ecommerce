using EdaMicroEcommerce.Domain.BuildingBlocks;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Order.Domain.Domain.Events;

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