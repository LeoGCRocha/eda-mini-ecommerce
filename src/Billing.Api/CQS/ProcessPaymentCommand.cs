using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;
using MediatR;

namespace Billing.Api.CQS;

public class ProcessPaymentCommand : IRequest<ProcessPaymentResponse>
{
    public PaymentId PaymentId { get; set; }
    public string? CouponName { get; set; }
}