using EdaMicroEcommerce.Domain.BuildingBlocks;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Orders.Domain.Entities.Events;

public class OrderItemRemovedEvent(OrderId orderId, ProductId productId, string? reason = null) : IDomainEvent
{
    public OrderId OrderId { get; set; } = orderId;
    public ProductId ProductId { get; set; } = productId;
    public string Reason { get; set; } = reason ?? string.Empty;
}