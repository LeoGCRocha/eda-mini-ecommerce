using MediatR;

namespace Orders.Api.CQS.CreateOrder;

public class CreateOrderCommand : IRequest
{
    public string CustomerId { get; set; }
    public List<OrderItemDto> OrderItemDtos { get; set; }

    public CreateOrderCommand(string customerId, List<OrderItemDto> orderItemDtos)
    {
        CustomerId = customerId;
        OrderItemDtos = orderItemDtos;
    }
}

public class OrderItemDto
{
    public Guid ProductId { get; set; }
    public int DesireQuantity { get; set; }

    public OrderItemDto(Guid productId, int desireQuantity)
    {
        ProductId = productId;
        DesireQuantity = desireQuantity;
    }
}