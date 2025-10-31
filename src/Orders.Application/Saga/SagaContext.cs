using Orders.Application.Saga.Entity;
using Orders.Domain.Entities;

namespace Orders.Application.Saga;

public class SagaContext
{
    public Order Order { get; set; }
    public SagaEntity? SagaEntity { get; set; }

    public SagaContext(Order order, SagaEntity? sagaEntity)
    {
        Order = order;
        SagaEntity = sagaEntity;
    }
}