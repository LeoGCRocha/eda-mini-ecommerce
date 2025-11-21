using Billing.Application.Strategy.Interfaces;
using Billing.Domain.Entities;

namespace Billing.Application.Strategy;

public class PaymentWithoutCouponStrategy : IPaymentStrategy
{
    public decimal ApplyDiscount(Payment payment, Coupon? coupon = null)
    {
        return payment.NetAmount;
    }
}