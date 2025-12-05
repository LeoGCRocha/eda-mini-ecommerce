using Billing.Domain.Entities;

namespace Billing.Application.Strategy.Interfaces;

public interface IPaymentStrategyFactory
{
    IPaymentStrategy GetPaymentStrategy(Coupon? coupon);
    IFeeStrategy GetFeeStrategy(Payment payment);
}