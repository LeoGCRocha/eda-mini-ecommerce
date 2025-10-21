using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Orders.Application.Saga;

public interface ISagaOrchestrator
{
    public Task ExecuteAsync<T>(OrderId orderId, T @event, CancellationToken cts = default);
}