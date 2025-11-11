namespace Orders.Application.Saga;

public interface ISagaStateHandlerFactory
{
    ISagaStateHandler<TEvent> GetHandler<TEvent>();
}