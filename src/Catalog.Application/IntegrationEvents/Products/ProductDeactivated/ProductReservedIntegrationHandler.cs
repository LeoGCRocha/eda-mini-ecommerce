using System.Text.Json;
using Catalog.Application.Observability;
using Catalog.Domain.Entities.InventoryItems.Events;
using EdaMicroEcommerce.Application.Outbox;
using MediatR;

namespace Catalog.Application.IntegrationEvents.Products.ProductDeactivated;

public class ProductReservedIntegrationHandler : IRequestHandler<ProductReservedIntegration>
{
    private readonly IIntegrationEventPublisher _eventPublisher;

    public ProductReservedIntegrationHandler(IIntegrationEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }
    
    public async Task Handle(ProductReservedIntegration request, CancellationToken cancellationToken)
    {
        var @object = JsonSerializer.Deserialize<ProductReservedEvent>(request.Payload);
        
        await _eventPublisher.PublishOnTopicAsync(@object, MessageBrokerConst.ProductReservedProducer);
    }
}