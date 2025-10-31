using System.Text.Json;
using Orders.Domain.Entities;
using Microsoft.Extensions.Logging;
using Orders.Domain.Entities.Events;
using Orders.Application.Saga.Entity;
using Orders.Application.Repositories;
using EdaMicroEcommerce.Application.Outbox;
using EdaMicroEcommerce.Domain.Enums;
using Orders.Application.IntegrationEvents;
using Orders.Application.IntegrationEvents.Products;
using Platform.SharedContracts.IntegrationEvents.Products;

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

        List<ProductReservationIntegration> productsEvents = [];
        
        productsEvents.AddRange(@event.ProductOrderInfos
            .Select(productReservation =>
                new ProductReservationEvent(@event.OrderId, productReservation.ProductId, productReservation.Quantity, ReservationEventType.Reservation))
            .Select(objectEvent =>
                new ProductReservationIntegration(EventType.ProductReservation,
                    JsonSerializer.Serialize(objectEvent))));

        return Task.FromResult(new SagaTransitionResult()
        {
            NewStatus = SagaStatus.PendingReservation,
            NewOrderStatus = OrderStatus.PendingReservation,
            ReferenceEntity = new SagaEntity(@event.OrderId, SagaStatus.OrderCreated),
            EventsToPublish = productsEvents.Cast<OutboxIntegrationEvent<EventType>>().ToList(), // Persistência dos eventos que irão para o outbox...
            IsChange = true
        });
    }
}