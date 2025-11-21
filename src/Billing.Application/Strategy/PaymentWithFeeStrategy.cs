using Billing.Application.Strategy.Interfaces;
using Billing.Domain.Entities;

namespace Billing.Application.Strategy;

public class PaymentWithFeeStrategy : IFeeStrategy
{
    public decimal ApplyFee(Payment payment, decimal feeTaxes)
    {
        if (feeTaxes == 0.0m)
            return payment.NetAmount;

        payment.ApplyFee(feeTaxes);

        return payment.NetAmount;
    }
}