using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Orders.Application.Saga.Entity;

public class SagaEntity
{
    public int Id { get; private set; }
    public OrderId OrderId { get; private set; }
    public object StateData { get; set; }
    public SagaStatus Status { get; set; }

    public SagaEntity(OrderId orderId, object? stateData, SagaStatus status)
    {
        OrderId = orderId;
        StateData = stateData;
        Status = status;
    }
}