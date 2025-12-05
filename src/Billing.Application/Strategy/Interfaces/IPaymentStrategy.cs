using Billing.Domain.Entities;

namespace Billing.Application.Strategy.Interfaces;

public interface IPaymentStrategy
{
    decimal ApplyDiscount(Payment payment, Coupon? coupon = null);
}