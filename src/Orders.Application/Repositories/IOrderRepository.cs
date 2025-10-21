using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;
using Orders.Domain.Entities;

namespace Orders.Application.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetOrderByIdAsync(OrderId order, CancellationToken cts = default);
}