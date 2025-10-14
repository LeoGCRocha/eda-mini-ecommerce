namespace EdaMicroEcommerce.Application.Outbox;

public interface IIntegrationEventPublisher
{
    Task PublishOnTopicAsync<T>(T payload, string producerName, string? key = null);
}