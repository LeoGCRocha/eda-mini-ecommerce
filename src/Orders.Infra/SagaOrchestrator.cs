using System.Diagnostics;
using Orders.Application;
using Orders.Domain.Entities;
using Orders.Application.Saga;
using Microsoft.Extensions.Logging;
using Orders.Application.Saga.Entity;
using Orders.Application.Repositories;
using Orders.Application.Observability;
using EdaMicroEcommerce.Domain.BuildingBlocks;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Orders.Infra;

public class SagaOrchestrator(
    OrderContext orderContext,
    ILogger<SagaOrchestrator> logger,
    IOrderRepository orderRepository,
    ISagaRepository sagaRepository,
    ISagaStateHandlerFactory stateHandlerFactory)
    : ISagaOrchestrator
{
    public async Task ExecuteAsync<T>(OrderId orderId, T @event, CancellationToken cts = default)
    {
        // 1. Buscar o contexto atual do SAGA
        using var activity = Source.OrderSource.StartActivity($"{nameof(SagaOrchestrator)} : Executing Orchestration", ActivityKind.Server);

        activity?.AddTag("order.id", orderId);
        activity?.AddTag("saga.event.type", typeof(T).Name);
        
        var context = await BuildContextAsync(orderId, cts);
        var handler = stateHandlerFactory.GetHandler<T>();

        activity?.AddTag("saga.start_status", context.SagaEntity?.Status.ToString());
        
        // 2. Validação se o handler e o status fazem sentido estarem juntos
        activity?.AddEvent(new ActivityEvent("Handler Compatibility Check"));
        
        if (!handler.CanHandle(context.SagaEntity?.Status))
        {
            logger.LogWarning(
                "Handler {HandlerType} não pode lidar com o evento no status {Status} para o pedido {OrderId}",
                handler.GetType().Name,
                context.SagaEntity?.Status,
                orderId);

            activity?.AddTag("saga.result", "SKIPPED_STATUS_MISMATCH");
            activity?.AddEvent(new ActivityEvent("Handler Skipped due to Status mismatch"));
            
            return;
        }

        // 3. Execução do handler realizando as operações esperadas
        // Obs.: Não existe commit ainda nesse momento então as mudanças não foram persistidas somente as lógicas foram feitas
        var result = await handler.HandleAsync(context, @event, cts);

        // 4. Nada foi aplicado e entrou nas regras de idempotência
        if (!result.IsChange)
            return;

        // 5. Se tiver mudanças deve realizar a transição de estados
        await ApplyAndTransitToNextState(context, result);

        activity?.AddTag("saga.end_status", context.SagaEntity?.Status.ToString());
    }

    private async Task ApplyAndTransitToNextState(SagaContext sagaContext, SagaTransitionResult sagaTransitionResult)
    {
        try
        {
            if (sagaTransitionResult.ReferenceEntity is not null)
            {
                sagaContext.SagaEntity = sagaTransitionResult.ReferenceEntity;
                await sagaRepository.AddAsync(sagaContext.SagaEntity);
            }
            
            if (sagaTransitionResult.NewOrderStatus is not null)
                sagaContext.Order.ChangeStatus((OrderStatus)sagaTransitionResult.NewOrderStatus);

            if (sagaTransitionResult.NewStatus is not null)
                if (sagaContext.SagaEntity != null)
                    sagaContext.SagaEntity.Status = (SagaStatus)sagaTransitionResult.NewStatus;

            if (sagaTransitionResult.EventsToPublish.Count > 0)
                await orderContext.OutboxIntegrationEvents.AddRangeAsync(sagaTransitionResult.EventsToPublish);

            await orderContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new GenericException($"Algo errado aconteceu durante a transição de estado, {ex.Message}");
        }
    }

    private async Task<SagaContext> BuildContextAsync(OrderId orderId, CancellationToken cts = default)
    {
        var orderObject = await orderRepository.GetOrderByIdAsync(orderId, cts);
        if (orderObject is null)
            throw new GenericException($"Não foi possível encontrar a Order: {orderId.Value}, saga invalido.");
        var sagaEntity = await sagaRepository.GetByOrderIdAsync(orderId, cts);

        return new SagaContext(orderObject, sagaEntity);
    }
}