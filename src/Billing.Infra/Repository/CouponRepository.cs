using Billing.Application.Repositories;
using Billing.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Billing.Infras.Repository;

public class CouponRepository : ICouponsRepository
{
    private readonly BillingContext _billingContext;

    public CouponRepository(BillingContext billingContext)
    {
        _billingContext = billingContext;
    }

    public async Task<Coupon?> GetLatestActiveCouponFromNameAsync(string name, CancellationToken cts = default)
    {
        return await _billingContext.Coupons.FirstOrDefaultAsync(coupon => coupon.Name == name && coupon.IsActive, cancellationToken: cts);
    }
}