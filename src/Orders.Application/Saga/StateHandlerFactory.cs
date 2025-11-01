using EdaMicroEcommerce.Domain.BuildingBlocks;
using Microsoft.Extensions.DependencyInjection;

namespace Orders.Application.Saga;

public class StateHandlerFactory : ISagaStateHandlerFactory
{
    private readonly IServiceProvider _serviceProvider;

    public StateHandlerFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ISagaStateHandler<TEvent> GetHandler<TEvent>()
    {
        var handler = _serviceProvider.GetService<ISagaStateHandler<TEvent>>();
        
        return handler ?? throw new GenericException($"Não foi encontrado o handler {typeof(TEvent).Name} pro saga.");
    }
}