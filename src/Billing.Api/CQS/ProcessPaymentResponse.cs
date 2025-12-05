using Billing.Domain.Entities;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Billing.Api.CQS;

public class ProcessPaymentResponse
{
    public PaymentId PaymentId { get; set; }
    public PaymentStatus PaymentStatus { get; set; }

    public ProcessPaymentResponse()
    {
        
    }
}