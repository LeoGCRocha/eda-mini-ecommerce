using Billing.Domain.Entities;

namespace Billing.Application.Repositories;

public interface ICouponsRepository
{
    public Task<Coupon?> GetLatestActiveCouponFromNameAsync(string name, CancellationToken cts = default);
}