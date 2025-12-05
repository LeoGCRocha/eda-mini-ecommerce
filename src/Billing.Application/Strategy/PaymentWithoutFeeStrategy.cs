using Billing.Application.Strategy.Interfaces;
using Billing.Domain.Entities;

namespace Billing.Application.Strategy;

public class PaymentWithoutFeeStrategy : IFeeStrategy
{
    public decimal ApplyFee(Payment payment, decimal feeTaxes)
    {
        return feeTaxes != 0
            ? throw new Exception("Unexpected fee received during a payment without fee strategy.")
            : payment.NetAmount;
    }
}