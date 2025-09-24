using EdaMicroEcommerce.Application.Outbox;

namespace EdaMicroEcommerce.Application.IntegrationEvents;

public interface IIntegrationEventPublisher
{
    Task PublishOnTopicAsync<T>(T @event) where T : OutboxIntegrationEvent;
}