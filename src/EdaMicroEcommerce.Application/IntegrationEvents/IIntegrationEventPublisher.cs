namespace EdaMicroEcommerce.Application.IntegrationEvents;

public interface IIntegrationEventPublisher
{
    Task PublishOnTopicAsync<T>(T payload, string producerName, string? key = null);
}