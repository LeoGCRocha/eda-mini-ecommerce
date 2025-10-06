using EdaMicroEcommerce.Domain.BuildingBlocks;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace EdaMicroEcommerce.Domain.Ordering.Events;

public class OrderCanceledEvent(OrderId orderId, string reason) : IDomainEvent
{
    public OrderId OrderId { get; set; } = orderId;
    public string Reason { get; set; } = reason;
}