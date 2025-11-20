using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Billing.Domain.Entities;

public class Payment
{
    public PaymentId Id { get; private set; }
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

    public Payment(decimal netAmount, decimal grossAmount, decimal feeAmount,
        decimal discountAmount, OrderId orderId, CustomerId customerId, string? discountReason = null)
    {
        Id = new PaymentId();
        Status = PaymentStatus.Created;
        NetAmount = netAmount;
        GrossAmount = grossAmount;
        FeeAmount = feeAmount;
        DiscountAmount = discountAmount;
        DiscountReason = discountReason;
        OrderId = orderId;
        CustomerId = customerId;
    }
}