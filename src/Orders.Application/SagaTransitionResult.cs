using Orders.Domain.Entities;
using Orders.Application.Saga.Entity;
using Orders.Application.IntegrationEvents;
using EdaMicroEcommerce.Application.Outbox;

namespace Orders.Application;

public class SagaTransitionResult
{
    public SagaStatus? NewStatus { get; set; }
    public OrderStatus? NewOrderStatus { get; set; }
    public SagaEntity? ReferenceEntity { get; set; }
    public List<OutboxIntegrationEvent<EventType>> EventsToPublish { get; set; } = [];
    public bool IsChange { get; set; } = false;

    public static SagaTransitionResult HasNoChange()
    {
        return new SagaTransitionResult() {};
    }
}