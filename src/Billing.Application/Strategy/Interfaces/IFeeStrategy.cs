using Billing.Domain.Entities;

namespace Billing.Application.Strategy.Interfaces;

public interface IFeeStrategy
{
    decimal ApplyFee(Payment payment, decimal feeTaxes = 0.0m);
}