using System.Text.Json;
using Catalog.Domain.Catalog.Products.Events;
using MediatR;

namespace Catalog.Application.IntegrationEvents.Products.ProductDeactivated;

public class ProductDeactivatedIntegrationHandler : IRequestHandler<ProductDeactivatedIntegration>
{
    private readonly IIntegrationEventPublisher _eventPublisher;

    public ProductDeactivatedIntegrationHandler(IIntegrationEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public Task Handle(ProductDeactivatedIntegration request, CancellationToken cancellationToken)
    {
        var @object = JsonSerializer.Deserialize<ProductDeactivatedEvent>(request.Payload);
        _eventPublisher.PublishOnTopicAsync(@object, MessageBrokerConst.ProductDeactivatedProducer, null);
        
        return Task.CompletedTask;
    }
}