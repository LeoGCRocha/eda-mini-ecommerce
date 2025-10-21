using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;
using Orders.Application.Saga.Entity;

namespace Orders.Application.Repositories;

public interface ISagaRepository
{
    Task<SagaEntity?> GetByOrderIdAsync(OrderId orderId, CancellationToken cts = default);
    Task AddAsync(SagaEntity sagaEntity, CancellationToken cts = default);
}