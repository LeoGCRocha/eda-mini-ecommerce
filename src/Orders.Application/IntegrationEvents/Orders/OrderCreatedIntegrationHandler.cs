using System.Text.Json;
using EdaMicroEcommerce.Application.Outbox;
using MediatR;
using Orders.Domain.Entities.Events;

namespace Orders.Application.IntegrationEvents.Orders;

public class OrderCreatedIntegrationHandler : IRequestHandler<OrderCreatedIntegration>
{
    private readonly IIntegrationEventPublisher _eventPublisher;

    public OrderCreatedIntegrationHandler(IIntegrationEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(OrderCreatedIntegration request, CancellationToken cancellationToken)
    {
        var @object = JsonSerializer.Deserialize<OrderCreatedEvent>(request.Payload);
        await _eventPublisher.PublishOnTopicAsync(@object, MessageBrokerConst.OrderCreatedProducer, null);
    }
}