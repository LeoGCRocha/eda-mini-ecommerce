using EdaMicroEcommerce.Domain.BuildingBlocks;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Platform.SharedContracts.IntegrationEvents.Payments;

public class PaymentProcessedEvent : IDomainEvent
{
    public PaymentId PaymentId { get; set; }
    public OrderId OrderId { get; set; }
    public string PaymentStatus { get; set; }

    public PaymentProcessedEvent(PaymentId paymentId, OrderId orderId, string paymentStatus)
    {
        PaymentId = paymentId;
        OrderId = orderId;
        PaymentStatus = paymentStatus;
    }
}