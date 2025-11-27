using Billing.Application.Strategy.Interfaces;
using Billing.Domain.Entities;

namespace Billing.Application.Strategy;

public class PaymentStrategyFactory : IPaymentStrategyFactory
{
    private readonly PaymentWithoutCouponStrategy _paymentWithoutCouponStrategy;
    private readonly PaymentWithCouponsStrategy _couponsStrategy;
    private readonly PaymentWithFeeStrategy _paymentWithFeeStrategy;
    private readonly PaymentWithoutFeeStrategy _paymentWithoutFeeStrategy;


    public PaymentStrategyFactory(PaymentWithoutCouponStrategy paymentWithoutCouponStrategy,
        PaymentWithCouponsStrategy couponsStrategy, PaymentWithFeeStrategy paymentWithFeeStrategy,
        PaymentWithoutFeeStrategy paymentWithoutFeeStrategy)
    {
        _paymentWithoutCouponStrategy = paymentWithoutCouponStrategy;
        _couponsStrategy = couponsStrategy;
        _paymentWithFeeStrategy = paymentWithFeeStrategy;
        _paymentWithoutFeeStrategy = paymentWithoutFeeStrategy;
    }

    public IPaymentStrategy GetPaymentStrategy(Coupon? coupon)
    {
        return coupon is null ? _paymentWithoutCouponStrategy : _couponsStrategy;
    }

    public IFeeStrategy GetFeeStrategy(Payment payment)
    {
        return _paymentWithFeeStrategy;
    }
}