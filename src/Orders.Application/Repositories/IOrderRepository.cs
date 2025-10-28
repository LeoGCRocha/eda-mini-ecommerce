using Orders.Domain.Entities;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;
using EdaMicroEcommerce.Domain.Enums;

namespace Orders.Application.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetOrderByIdAsync(OrderId order, CancellationToken cts = default);
    List<(ProductId, int)> CancelAllReservationFromAFailure(Order order, ProductId productId);
    Task CommitAsync();
    void UpdateOrderItemStatus(Order order, ProductId productId);
}