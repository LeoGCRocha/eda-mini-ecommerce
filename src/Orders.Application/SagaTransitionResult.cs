using EdaMicroEcommerce.Application.Outbox;
using Orders.Application.IntegrationEvents;
using Orders.Application.Saga.Entity;
using Orders.Application.Saga.States;
using Orders.Domain.Entities;

namespace Orders.Application;

public class SagaTransitionResult
{
    public SagaStatus? NewStatus { get; set; }
    public OrderStatus? NewOrderStatus { get; set; }
    public SagaEntity? ReferenceEntity { get; set; }
    public List<OutboxIntegrationEvent<EventType>> EventsToPublish { get; set; } = [];
    public bool IsChange { get; set; } = false;

    public StateData? UpdatedStateData { get; set; }

    public static SagaTransitionResult HasNoChange()
    {
        return new SagaTransitionResult() {};
    }
}