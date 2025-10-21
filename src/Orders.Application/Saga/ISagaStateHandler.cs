using Orders.Application.Saga.Entity;

namespace Orders.Application.Saga;

public interface ISagaStateHandler<in TEvent>
{
    bool CanHandle(SagaStatus? status);
    Task<SagaTransitionResult> HandleAsync(SagaContext context, TEvent @event, CancellationToken cts = default);
}