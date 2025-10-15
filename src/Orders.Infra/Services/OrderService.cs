using Orders.Domain.Entities;
using Orders.Domain.Services;

namespace Orders.Infra.Services;

public class OrderService : IOrderService
{
    private readonly OrderContext _orderContext;

    public OrderService(OrderContext orderContext)
    {
        _orderContext = orderContext;
    }

    public async Task CreateOrderAsync(Order orderToCreate)
    {
        await _orderContext.Orders.AddAsync(orderToCreate);
        await _orderContext.SaveChangesAsync();
    }
}