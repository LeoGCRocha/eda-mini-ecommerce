using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Orders.Application.Saga.Entity;

public class SagaEntity
{
    public int Id { get; private set; }
    public OrderId OrderId { get; private set; }
    public SagaStatus Status { get; set; }

    public SagaEntity(OrderId orderId, SagaStatus status)
    {
        OrderId = orderId;
        Status = status;
    }
}