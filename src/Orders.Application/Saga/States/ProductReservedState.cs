using System.Text.Json;
using EdaMicroEcommerce.Application.Outbox;
using EdaMicroEcommerce.Domain.BuildingBlocks;
using EdaMicroEcommerce.Domain.Enums;
using Microsoft.Extensions.Logging;
using Orders.Application.IntegrationEvents;
using Orders.Application.IntegrationEvents.Payments;
using Orders.Application.IntegrationEvents.Products;
using Orders.Application.Observability;
using Orders.Application.Repositories;
using Orders.Application.Saga.Entity;
using Orders.Domain.Entities;
using Orders.Domain.Entities.Events;
using Platform.SharedContracts.IntegrationEvents.Payments;
using Platform.SharedContracts.IntegrationEvents.Products;

namespace Orders.Application.Saga.States;

public class ProductReservedState(ILogger<ProductReservedState> logger, IOrderRepository orderRepository)
    : ISagaStateHandler<ProductReservationStatusEvent>
{
    public bool CanHandle(SagaStatus? status)
    {
        return status is SagaStatus.PendingReservation or SagaStatus.FailedReservation;
    }

    // WARNING: A forma que foi feito o code sempre ta tendo muitas roundtrips no banco talvez daria pra juntar varias coisas de uma vez so
    public async Task<SagaTransitionResult> HandleAsync(SagaContext context, ProductReservationStatusEvent @event,
        CancellationToken cts = default)
    {
        // TODO: Esse evento em específico não ta registrando o tipo no OUTBOX causando um ruído no FLUXO....
        using var activity =
            Source.OrderSource.StartActivity(
                $"{nameof(ProductReservedState)} : Responding to a product reservation on the order");

        activity?.AddTag("saga.handler.type", nameof(ProductReservationEvent));

        var entity = context.SagaEntity;
        Order order = context.Order;

        if (entity is null)
            // WARNING: Esse pode não ser o melhor caminho para lidar por que ficaria perna faltando
            throw new GenericException(
                $"Saga should be defined before a reservation event, on OrderId({@event.OrderId})");

        // Evitando comportamento inadequado ou dupla tentativa de reserva de produto
        var reservationStatus = order.OrderItems.First(or => or.ProductId == @event.ProductId).ReservationStatus;

        activity?.AddTag("saga.reservation.status", reservationStatus.ToString());

        if (reservationStatus is not ReservationStatus.Pending)
        {
            logger.LogWarning(
                "Reservation event was thrown twice **OR** invalid state found in the SAGA, for the OrderId ({OrderId})",
                @event.OrderId);
            return SagaTransitionResult.HasNoChange();
        }

        if (@event.ReservationEventType is ReservationEventType.Failure)
            return await ReservationCancellationCompensation(context, @event);

        if (entity.Status == SagaStatus.FailedReservation)
        {
            // Chegou uma reserva para o pedido no SAGA, porém ele já tinha obtido erro em outra
            List<ProductReservationIntegration> reservationFailedIntegrations =
            [
                // Cancelar a reserva desse pedido
                new(EventType.ProductReservation,
                    JsonSerializer.Serialize(new ProductReservationEvent(@event.OrderId, @event.ProductId,
                        @event.ReservedQuantity, ReservationEventType.Cancellation)))
            ];

            orderRepository.UpdateOrderItemStatus(context.Order, @event.ProductId);

            return new SagaTransitionResult()
            {
                EventsToPublish = reservationFailedIntegrations.Cast<OutboxIntegrationEvent<EventType>>().ToList(),
                IsChange = true
            };
        }

        // Caminho normal esperado
        // Realizar reserva recebida do produto no pedido em específico.
        if (@event.ReservationEventType is not ReservationEventType.Reservation)
            throw new GenericException($"Tipo de evento inesperado para o pedido ({@event.OrderId}) neste momento.");

        // Atualiza o estado do produto atual no pedido
        order.UpdateOrderItensStatus([@event.ProductId], ReservationStatus.Reserved);

        if (order.OrderItems.Where(or => or.ReservationStatus == ReservationStatus.Reserved).ToList().Count ==
            order.OrderItems.Count)
        {
            // If all itens were already reserve we should proceed to next valid state
            List<PaymentPendingIntegration> paymentPendingIntegrations =
            [
                new PaymentPendingIntegration(EventType.PaymentPending, JsonSerializer.Serialize(
                    new PaymentPendingEvent(context.Order.Id, context.Order.TotalAmount, context.Order.CustomerId)
                ))
            ];

            return new SagaTransitionResult()
            {
                NewStatus = SagaStatus.PendingPayment,
                EventsToPublish = paymentPendingIntegrations.Cast<OutboxIntegrationEvent<EventType>>().ToList(),
                NewOrderStatus = OrderStatus.PendingPayment,
                IsChange = true
            };
        }

        return new SagaTransitionResult()
        {
            IsChange = true
        };
    }

    private async Task<SagaTransitionResult> ReservationCancellationCompensation(SagaContext context,
        ProductReservationStatusEvent @event)
    {
        // Ao chegar um evento de falha para um produto específico na SAGA, os demais produtos do pedido
        // precisarão ser cancelados e compensados
        try
        {
            context.Order.ChangeStatus(OrderStatus.FailedReservation);
            context.SagaEntity!.Status = SagaStatus.FailedReservation;
        }
        catch (Exception ex)
        {
            // !!!! <WARNING>
            // Aqui deveríamos ter um comportamento para lidar com essa falha, pois representa um estado critico para o sistema
            // Caso isso aqui não aconteça o estoque vai ficar travado sem um pedido realmente associado.
            logger.LogError(ex, "Falha ao tentar compensar o evento para {OrderId}.", @event.OrderId);
            throw;
        }

        List<ProductReservationIntegration> reservationToCancel = [];

        var reservationToCancelTuple =
            orderRepository.CancelAllReservationFromAFailure(context.Order, @event.ProductId);

        reservationToCancel.AddRange(reservationToCancelTuple.Select(tuple =>
            new ProductReservationIntegration(EventType.ProductReservation,
                JsonSerializer.Serialize(new ProductReservationEvent(@event.OrderId, tuple.Item1,
                    tuple.Item2, ReservationEventType.Cancellation)))));

        return await Task.FromResult(new SagaTransitionResult
        {
            NewStatus = SagaStatus.FailedReservation,
            NewOrderStatus = OrderStatus.FailedReservation,
            EventsToPublish =
                reservationToCancel.Cast<OutboxIntegrationEvent<EventType>>()
                    .ToList(), // Envia pro outbox e depois ele envia para o topic interessado
            IsChange = true
        });
    }
}