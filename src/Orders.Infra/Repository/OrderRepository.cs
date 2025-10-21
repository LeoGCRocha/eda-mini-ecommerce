using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;
using Microsoft.EntityFrameworkCore;
using Orders.Application.Repositories;
using Orders.Domain.Entities;

namespace Orders.Infra.Repository;

public class OrderRepository(OrderContext orderContext) : IOrderRepository
{
    public async Task<Order?> GetOrderByIdAsync(OrderId order, CancellationToken cts = default)
    {
        return await orderContext.Orders.FirstOrDefaultAsync(or => or.Id == order, cts);
    }
}