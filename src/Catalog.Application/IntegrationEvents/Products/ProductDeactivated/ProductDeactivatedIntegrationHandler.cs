using MediatR;
using System.Text.Json;
using Catalog.Application.Observability;
using EdaMicroEcommerce.Application.Outbox;
using Catalog.Domain.Entities.Products.Events;

namespace Catalog.Application.IntegrationEvents.Products.ProductDeactivated;

public class ProductDeactivatedIntegrationHandler(IIntegrationEventPublisher eventPublisher)
    : IRequestHandler<ProductDeactivatedIntegration>
{
    public async Task Handle(ProductDeactivatedIntegration request, CancellationToken cancellationToken)
    {
        using var activity = Source.CatalogSource.StartActivity($"{nameof(ProductDeactivatedIntegrationHandler)} : Sending message through broker.");
        
        var @object = JsonSerializer.Deserialize<ProductDeactivatedEvent>(request.Payload);
        await eventPublisher.PublishOnTopicAsync(@object, MessageBrokerConst.ProductDeactivatedProducer);
    }
}