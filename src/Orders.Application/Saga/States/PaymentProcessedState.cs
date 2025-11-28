using System.Text.Json;
using Billing.Domain.Entities;
using EdaMicroEcommerce.Application.Outbox;
using EdaMicroEcommerce.Domain.BuildingBlocks;
using EdaMicroEcommerce.Domain.Enums;
using Microsoft.Extensions.Logging;
using Orders.Application.IntegrationEvents;
using Orders.Application.IntegrationEvents.Products;
using Orders.Application.Observability;
using Orders.Application.Saga.Entity;
using Orders.Domain.Entities;
using Platform.SharedContracts.IntegrationEvents.Payments;
using Platform.SharedContracts.IntegrationEvents.Products;

namespace Orders.Application.Saga.States;

public class PaymentProcessedState(ILogger<PaymentProcessedState> logger)
    : ISagaStateHandler<PaymentProcessedEvent>
{
    public bool CanHandle(SagaStatus? status)
    {
        return status is SagaStatus.PendingPayment;
    }

    public async Task<SagaTransitionResult> HandleAsync(SagaContext context, PaymentProcessedEvent @event,
        CancellationToken cts = default)
    {
        using var activity =
            Source.OrderSource.StartActivity(
                $"{nameof(PaymentProcessedState)} : Responding to a payment processed event");

        activity?.AddTag("saga.handler.type", nameof(PaymentProcessedEvent));

        var entity = context.SagaEntity;
        Order order = context.Order;

        if (entity is null)
            throw new GenericException(
                $"Saga should be defined before a reservation event, on OrderId({@event.OrderId})");

        PaymentStatus paymentStatus;

        try
        {
            paymentStatus = (PaymentStatus)Enum.Parse(typeof(PaymentStatus), @event.PaymentStatus);
        }
        catch (ArgumentException ex)
        {
            logger.LogError(ex, "Unexpected paymentStatus received.");
            throw;
        }

        List<ProductReservationIntegration> productReservationIntegrations = [];

        if (paymentStatus == PaymentStatus.Approved)
        {
            var reservationEvents = order.OrderItems.Select(or =>
                    new ProductReservationEvent(order.Id, or.ProductId, or.Quantity, ReservationEventType.Confirmation))
                .ToList();

            foreach (var reservationEvt in reservationEvents)
                productReservationIntegrations.Add(new(EventType.ProductReservation,
                    JsonSerializer.Serialize(reservationEvt)));

            return new SagaTransitionResult()
            {
                NewStatus = SagaStatus.Finished,
                NewOrderStatus = OrderStatus.Paid,
                EventsToPublish = productReservationIntegrations.Cast<OutboxIntegrationEvent<EventType>>().ToList(),
                IsChange = true
            };
        }

        if (paymentStatus == PaymentStatus.Refused)
        {
            var reservationEvents = order.OrderItems.Select(or =>
                    new ProductReservationEvent(order.Id, or.ProductId, or.Quantity, ReservationEventType.Cancellation))
                .ToList();

            foreach (var reservationEvt in reservationEvents)
                productReservationIntegrations.Add(new(EventType.ProductReservation,
                    JsonSerializer.Serialize(reservationEvt)));

            return new SagaTransitionResult()
            {
                NewStatus = SagaStatus.Failed,
                NewOrderStatus = OrderStatus.Canceled,
                EventsToPublish = productReservationIntegrations.Cast<OutboxIntegrationEvent<EventType>>().ToList(),
                IsChange = true
            };
        }

        throw new Exception("An unexpected payment status was found.");
    }
}