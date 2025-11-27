using Billing.Domain.Entities.Events;
using EdaMicroEcommerce.Domain.BuildingBlocks;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Billing.Domain.Entities;

public class Payment : AggregateRoot<PaymentId>
{
    public PaymentStatus Status { get; private set; }
    public decimal NetAmount { get; private set; }
    public decimal GrossAmount { get; private set; }
    public decimal FeeAmount { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public string? DiscountReason { get; private set; }
    public OrderId OrderId { get; set; }
    public CustomerId CustomerId { get; set; }

    private Payment()
    {
    }

    public Payment(decimal grossAmount, OrderId orderId, CustomerId customerId)
    {
        Id = new PaymentId();
        Status = PaymentStatus.Created;
        NetAmount = grossAmount;
        GrossAmount = grossAmount;
        FeeAmount = 0;
        DiscountAmount = 0;
        DiscountReason = null;
        OrderId = orderId;
        CustomerId = customerId;
    }

    public void ApplyDiscount(decimal discountValue, string discountReason)
    {
        NetAmount = GrossAmount - discountValue;
        DiscountReason = discountReason;
        DiscountAmount = discountValue;
    }

    public void ApplyFee(decimal feeTax)
    {
        NetAmount -= Math.Abs(feeTax);
    }

    public void Process(PaymentStatus status)
    {
        if (Status != PaymentStatus.Created)
            throw new Exception("Payment already have a state.");

        Status = status;
        AddDomainEvent(new PaymentProcessedEvent(Id, OrderId, Status.ToString()));
    }
}