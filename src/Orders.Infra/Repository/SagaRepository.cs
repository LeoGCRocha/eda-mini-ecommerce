using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;
using Microsoft.EntityFrameworkCore;
using Orders.Application.Repositories;
using Orders.Application.Saga.Entity;

namespace Orders.Infra.Repository;

public class SagaRepository(OrderContext orderContext) : ISagaRepository
{
    public async Task<SagaEntity?> GetByOrderIdAsync(OrderId orderId, CancellationToken cts = default)
    {
        return await orderContext.Saga.FirstOrDefaultAsync(or => or.OrderId == orderId, cancellationToken: cts);
    }

    public async Task AddAsync(SagaEntity sagaEntity, CancellationToken cts = default)
    {
        await orderContext.Saga.AddAsync(sagaEntity, cancellationToken: cts);
    }
}