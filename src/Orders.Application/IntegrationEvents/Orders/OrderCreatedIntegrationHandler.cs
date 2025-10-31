using MediatR;
using System.Text.Json;
using Orders.Domain.Entities.Events;
using EdaMicroEcommerce.Application.Outbox;

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