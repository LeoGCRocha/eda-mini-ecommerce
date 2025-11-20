using Billing.Domain.Entities;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Billing.Application.Repositories;

public interface IPaymentRepository
{
    public Task<Payment?> GetPaymentFromOrderIdAsync(OrderId orderId, CancellationToken cts = default);
    public Task AddPaymentAsync(Payment payment, CancellationToken cts = default);
}