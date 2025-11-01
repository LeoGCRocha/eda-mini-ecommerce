using MediatR;
using System.Text.Json;
using EdaMicroEcommerce.Application.Outbox;
using Catalog.Domain.Entities.Products.Events;

namespace Catalog.Application.IntegrationEvents.Products.ProductDeactivated;

public class ProductDeactivatedIntegrationHandler(IIntegrationEventPublisher eventPublisher)
    : IRequestHandler<ProductDeactivatedIntegration>
{
    public async Task Handle(ProductDeactivatedIntegration request, CancellationToken cancellationToken)
    {
        var @object = JsonSerializer.Deserialize<ProductDeactivatedEvent>(request.Payload);
        await eventPublisher.PublishOnTopicAsync(@object, MessageBrokerConst.ProductDeactivatedProducer);
    }
}