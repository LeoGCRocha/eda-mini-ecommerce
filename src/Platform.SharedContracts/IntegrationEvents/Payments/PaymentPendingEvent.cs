using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Platform.SharedContracts.IntegrationEvents.Payments;

public class PaymentPendingEvent(OrderId orderId, decimal totalAmount, CustomerId customerId)
{
    public OrderId OrderId { get; init; } = orderId;
    public decimal TotalAmount { get; init; } = totalAmount;
    public CustomerId CustomerId { get; init; } = customerId;
}