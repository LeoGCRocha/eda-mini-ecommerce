using Orders.Domain.Entities;

namespace Orders.Domain.Services;

public interface IOrderService
{
    public Task CreateOrderAsync(Order orderToCreate);
}