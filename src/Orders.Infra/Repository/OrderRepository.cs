using Orders.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using EdaMicroEcommerce.Domain.Enums;
using Orders.Application.Repositories;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Orders.Infra.Repository;

public class OrderRepository(OrderContext orderContext) : IOrderRepository
{
    public async Task<Order?> GetOrderByIdAsync(OrderId order, CancellationToken cts = default)
    {
        return await orderContext.Orders.Include(or => or.OrderItems).FirstOrDefaultAsync(or => or.Id == order, cts);
    }

    public List<(ProductId, int)> CancelAllReservationFromAFailure(Order order, ProductId productId)
    {
        order.UpdateOrderItensStatus([productId], ReservationStatus.Failed);

        return order.UpdateOrderItensStatus(
            order.OrderItems.Where(or => or.ReservationStatus == ReservationStatus.Reserved)
                .Select(or => or.ProductId).ToList(), ReservationStatus.Cancelled);
    }

    public void UpdateOrderItemStatus(Order order, ProductId productId)
    {
        order.UpdateOrderItensStatus([productId], ReservationStatus.Cancelled);
    }

    public async Task CommitAsync()
    {
        await orderContext.SaveChangesAsync();
    }
}