using Orders.Application;
using Orders.Domain.Entities;
using Orders.Application.Saga;
using Microsoft.Extensions.Logging;
using Orders.Application.Saga.Entity;
using Orders.Application.Repositories;
using EdaMicroEcommerce.Domain.BuildingBlocks;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Orders.Infra;

public class SagaOrchestrator : ISagaOrchestrator
{
    private readonly OrderContext _orderContext;
    private readonly ILogger<SagaOrchestrator> _logger;
    private readonly IOrderRepository _orderRepository;
    private readonly ISagaRepository _sagaRepository;
    private readonly ISagaStateHandlerFactory _stateHandlerFactory;

    public SagaOrchestrator(OrderContext orderContext, ILogger<SagaOrchestrator> logger,
        IOrderRepository orderRepository, ISagaRepository sagaRepository, ISagaStateHandlerFactory stateHandlerFactory)
    {
        _orderContext = orderContext;
        _logger = logger;
        _orderRepository = orderRepository;
        _sagaRepository = sagaRepository;
        _stateHandlerFactory = stateHandlerFactory;
    }

    public async Task ExecuteAsync<T>(OrderId orderId, T @event, CancellationToken cts = default)
    {
        // 1. Buscar o contexto atual do SAGA
        var context = await BuildContextAsync(orderId, cts);
        var handler = _stateHandlerFactory.GetHandler<T>();

        // 2. Validação se o handler e o status fazem sentido estarem juntos
        if (!handler.CanHandle(context.SagaEntity?.Status))
        {
            _logger.LogWarning(
                "Handler {HandlerType} não pode lidar com o evento no status {Status} para o pedido {OrderId}",
                handler.GetType().Name,
                context.SagaEntity?.Status,
                orderId);
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
    }

    private async Task ApplyAndTransitToNextState(SagaContext sagaContext, SagaTransitionResult sagaTransitionResult)
    {
        try
        {
            if (sagaTransitionResult.ReferenceEntity is not null)
            {
                sagaContext.SagaEntity = sagaTransitionResult.ReferenceEntity;
                await _sagaRepository.AddAsync(sagaContext.SagaEntity);
            }
            
            if (sagaTransitionResult.NewOrderStatus is not null)
                sagaContext.Order.ChangeStatus((OrderStatus)sagaTransitionResult.NewOrderStatus);

            if (sagaTransitionResult.NewStatus is not null)
                if (sagaContext.SagaEntity != null)
                    sagaContext.SagaEntity.Status = (SagaStatus)sagaTransitionResult.NewStatus;

            if (sagaTransitionResult.EventsToPublish.Count > 0)
                await _orderContext.OutboxIntegrationEvents.AddRangeAsync(sagaTransitionResult.EventsToPublish);

            await _orderContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new GenericException($"Algo errado aconteceu durante a transição de estado, {ex.Message}");
        }
    }

    private async Task<SagaContext> BuildContextAsync(OrderId orderId, CancellationToken cts = default)
    {
        var orderObject = await _orderRepository.GetOrderByIdAsync(orderId, cts);
        if (orderObject is null)
            throw new GenericException($"Não foi possível encontrar a Order: {orderId.Value}, saga invalido.");
        var sagaEntity = await _sagaRepository.GetByOrderIdAsync(orderId, cts);

        return new SagaContext(orderObject, sagaEntity);
    }
}