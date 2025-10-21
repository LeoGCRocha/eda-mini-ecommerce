using System.Text.Json;
using Orders.Domain.Entities;
using Microsoft.Extensions.Logging;
using Orders.Domain.Entities.Events;
using Orders.Application.Saga.Entity;
using Orders.Application.Repositories;
using EdaMicroEcommerce.Application.Outbox;
using Orders.Application.IntegrationEvents;
using Orders.Application.IntegrationEvents.Products;

namespace Orders.Application.Saga.States;

public class OrderCreatedState(ILogger<OrderCreatedState> logger, ISagaRepository sagaRepository) : ISagaStateHandler<OrderCreatedEvent>
{
    public bool CanHandle(SagaStatus? status)
    {
        return status is null;
    }

    public Task<SagaTransitionResult> HandleAsync(SagaContext context, OrderCreatedEvent @event,
        CancellationToken cts = default)
    {
        if (context.SagaEntity is not null)
        {
            logger.LogWarning("Tentativa duplicada da execução para a Order {OrderId}",
                @event.OrderId);
            return Task.FromResult(SagaTransitionResult.HasNoChange());
        }

        var stateData = new StateData()
        {
            ExpectedReservations = @event.ProductOrderInfos.Count,
            CurrentReservations = 0,
            FailedReservations = 0,
            AlreadyReserved = []
        };

        List<ProductReservationIntegration> productsEvents = [];
        
        productsEvents.AddRange(@event.ProductOrderInfos
            .Select(productReservation =>
                new ProductReservationEvent(@event.OrderId, productReservation.ProductId, productReservation.Quantity, ReservationType.RESERVATION))
            .Select(objectEvent =>
                new ProductReservationIntegration(EventType.ProductReservation,
                    JsonSerializer.Serialize(objectEvent))));

        return Task.FromResult(new SagaTransitionResult()
        {
            NewStatus = SagaStatus.PENDING_RESERVATION,
            NewOrderStatus = OrderStatus.PENDING_RESERVATION,
            ReferenceEntity = new SagaEntity(@event.OrderId, JsonSerializer.Serialize(stateData), SagaStatus.ORDER_CREATED),
            EventsToPublish = productsEvents.Cast<OutboxIntegrationEvent<EventType>>().ToList(), // Persistência dos eventos que irão para o outbox...
            UpdatedStateData = stateData,
            IsChange = true
        });
    }
}