namespace Orders.Api.CQS.CreateOrder;

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