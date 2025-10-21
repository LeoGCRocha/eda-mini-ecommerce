using System.Text.Json;
using EdaMicroEcommerce.Application.Outbox;
using EdaMicroEcommerce.Domain.BuildingBlocks;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;
using Microsoft.Extensions.Logging;
using Orders.Application.IntegrationEvents;
using Orders.Application.IntegrationEvents.Products;
using Orders.Application.Saga.Entity;
using Orders.Domain.Entities;
using Orders.Domain.Entities.Events;

namespace Orders.Application.Saga.States;

public class ProductReservedState(ILogger<ProductReservedState> logger) : ISagaStateHandler<ProductReservedEvent>
{
    public bool CanHandle(SagaStatus? status)
    {
        return status == SagaStatus.PENDING_RESERVATION;
    }

    public Task<SagaTransitionResult> HandleAsync(SagaContext context, ProductReservedEvent @event,
        CancellationToken cts = default)
    {
        // <WARNING> Importante da forma que foi feito aqui se tiver um paralelismo para um mesmo pedido
        // poderia quebrar e a falha não compensar corretamente, além de permitir estados intermitentes
        var entity = context.SagaEntity;
        if (entity?.StateData is null)
            throw new GenericException("Espera-se um estado definido para este evento no saga.");

        var stateData = JsonSerializer.Deserialize<StateData>(entity.StateData as string ?? string.Empty);

        if (stateData!.AlreadyReserved.FirstOrDefault(prod => prod.ProductId == @event.ProductId) is not null)
        {
            logger.LogWarning("Produto {ProductId} já esta reservado para o Pedido {OrderId}", @event.ProductId,
                @event.OrderId);
            return Task.FromResult(SagaTransitionResult.HasNoChange());
        }

        if (!@event.IsReservationSucceed) // Compensação evento de FALHA [Produto não pode ser reservado]
            return Task.FromResult(CompensationFromAFailure(context, @event, stateData));

        // TODO: Testar isso aqui
        if (entity.Status == SagaStatus.FAILED_RESERVATION)
        {
            // Se conseguiu reservar mas já estava num estado de falha deve cancelar a reserva
            List<ProductReservationIntegration> reservationFailedIntegrations =
            [
                new(EventType.ProductReservation,
                    JsonSerializer.Serialize(new ProductReservationEvent(@event.OrderId, @event.ProductId,
                        @event.ReservedQuantity,  ReservationType.CANCELLATION)))

            ];

            return Task.FromResult(new SagaTransitionResult()
            {
                EventsToPublish = reservationFailedIntegrations.Cast<OutboxIntegrationEvent<EventType>>().ToList(),
                // Compensação publicada deve ser enviado para o topico que cancela as reservas
                IsChange = true
            });
        }
        
        stateData.CurrentReservations++;
        stateData.AlreadyReserved.Add(new ProductInformation()
        {
            ProductId = @event.ProductId,
            Quantity = @event.ReservedQuantity
        });
        entity.StateData = JsonSerializer.Serialize(stateData);

        if (stateData.CurrentReservations != stateData.ExpectedReservations)
            return Task.FromResult(new SagaTransitionResult()
            {
                IsChange = true
            });

        entity.Status = SagaStatus.PENDING_PAYMENT;
        context.Order.ChangeStatus(OrderStatus.PENDING_PAYMENT);

        return Task.FromResult(new SagaTransitionResult
        {
            NewStatus = SagaStatus.PENDING_PAYMENT,
            NewOrderStatus = OrderStatus.PENDING_PAYMENT,
            IsChange = true
        });
    }

    private SagaTransitionResult CompensationFromAFailure(SagaContext context, ProductReservedEvent @event,
        StateData stateData)
    {
        stateData.FailedReservations++;

        context.SagaEntity!.StateData = JsonSerializer.Serialize(stateData);
        context.Order.ChangeStatus(OrderStatus.FAILED_RESERVATION);
        context.SagaEntity!.Status = SagaStatus.FAILED_RESERVATION;

        List<ProductReservationIntegration> reservationToCancel = [];

        // TODO: Testar se isso aqui ta funcionando deve cancelar as reservas feitas do produtos anteriormnete.
        // TODO: Com esses dois pontos finalizados esta etapa do saga ta concluindo
        // TODO: Criar talvez um objeto no saga Already canceled pra evitar duplicade pra tentar cancelar reserva
        var stateDate = (StateData)context.SagaEntity.StateData;
        reservationToCancel.AddRange(
            stateDate.AlreadyReserved.Select(prod => 
                new ProductReservationIntegration(EventType.ProductReservation, 
                    JsonSerializer.Serialize(new ProductReservationEvent(context.Order.Id, prod.ProductId, prod.Quantity, ReservationType.CANCELLATION)))));

        return new SagaTransitionResult
        {
            NewStatus = SagaStatus.FAILED_RESERVATION,
            NewOrderStatus = OrderStatus.FAILED_RESERVATION,
            EventsToPublish = reservationToCancel.Cast<OutboxIntegrationEvent<EventType>>().ToList(),
            IsChange = true
        };
    }
}