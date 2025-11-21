using Billing.Application.Strategy.Interfaces;
using Billing.Domain.Entities;

namespace Billing.Application.Strategy;

public class PaymentWithCouponsStrategy : IPaymentStrategy
{
    public decimal ApplyDiscount(Payment payment, Coupon? coupon = null)
    {
        if (coupon is null)
            throw new Exception("When a payment with a coupon is made, the coupon is mandatory.");

        decimal discountValue = payment.GrossAmount * coupon.DiscountPercentage;
        
        payment.ApplyDiscount(discountValue, $"{coupon.Name} applied");

        return payment.NetAmount;
    }
}