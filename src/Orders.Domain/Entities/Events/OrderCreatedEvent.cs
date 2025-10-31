using EdaMicroEcommerce.Domain.BuildingBlocks;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Orders.Domain.Entities.Events;

public class OrderCreatedEvent : IDomainEvent
{
    public OrderId OrderId { get; set; }
    public List<ProductOrderInfo> ProductOrderInfos { get; set; }

    public OrderCreatedEvent(OrderId orderId, List<ProductOrderInfo> productOrderInfos)
    {
        OrderId = orderId;
        ProductOrderInfos = productOrderInfos;
    }
}