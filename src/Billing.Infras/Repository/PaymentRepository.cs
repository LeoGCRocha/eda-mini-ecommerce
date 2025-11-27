using Billing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Billing.Application.Repositories;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Billing.Infras.Repository;

public class PaymentRepository : IPaymentRepository
{
    private BillingContext BillingContext { get; set; }

    public PaymentRepository(BillingContext billingContext)
    {
        BillingContext = billingContext;
    }
    
    public async Task<Payment?> GetPaymentFromOrderIdAsync(OrderId orderId, CancellationToken cts = default)
    {
        return await BillingContext.Payments.FirstOrDefaultAsync(payment => payment.OrderId == orderId, cts);
    }

    public async Task<Payment?> GetPaymentFromId(PaymentId paymentId, CancellationToken cts = default)
    {
        return await BillingContext.Payments.FirstOrDefaultAsync(payment => payment.Id == paymentId, cts);
    }

    public async Task AddPaymentAsync(Payment payment, CancellationToken cts = default)
    {
        await BillingContext.AddRangeAsync(payment);
        await BillingContext.SaveChangesAsync(cts);
    }

    public async Task SaveAsync(CancellationToken cts = default)
    {
        await BillingContext.SaveChangesAsync(cts);
    }
}