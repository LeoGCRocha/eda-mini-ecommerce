using Billing.Domain.Entities;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Billing.Application.Services;

public interface IPaymentCouponService
{
    public Task<Payment> ProcessPayment(PaymentId paymentId, string? couponName = default);
}