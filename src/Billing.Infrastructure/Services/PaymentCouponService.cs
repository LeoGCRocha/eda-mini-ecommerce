using Billing.Application.Services;
using Billing.Application.Strategy;
using Billing.Application.Repositories;
using Billing.Application.Strategy.Interfaces;
using Billing.Domain.Entities;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;
using Microsoft.Extensions.DependencyInjection;

namespace Billing.Infrastructure.Services;

public class PaymentCouponService : IPaymentCouponService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ICouponsRepository _couponsRepository;
    private readonly IServiceProvider _serviceProvider;

    public PaymentCouponService(IPaymentRepository paymentRepository, ICouponsRepository couponsRepository,
        IServiceProvider serviceProvider)
    {
        _paymentRepository = paymentRepository;
        _couponsRepository = couponsRepository;
        _serviceProvider = serviceProvider;
    }

    public async Task<Payment> ProcessPayment(PaymentId paymentId,  string? couponName = default)
    {
        var payment = await _paymentRepository.GetPaymentFromId(paymentId);

        if (payment is null)
            throw new Exception($"PaymentId {paymentId.Value} was not found");
        
        var coupon = (couponName is null) ? null : await _couponsRepository.GetLatestActiveCouponFromNameAsync(couponName);

        if (coupon is null && couponName is not null)
            throw new Exception($"{couponName} was not found.");

        IPaymentStrategy paymentStrategy = (coupon is null)
            ? _serviceProvider.GetRequiredService<PaymentWithoutCouponStrategy>()
            : _serviceProvider.GetRequiredService<PaymentWithCouponsStrategy>();

        paymentStrategy.ApplyDiscount(payment, coupon);

        IFeeStrategy feeStrategy = _serviceProvider.GetRequiredService<PaymentWithoutFeeStrategy>();

        feeStrategy.ApplyFee(payment);
        
        return payment;
    }
}