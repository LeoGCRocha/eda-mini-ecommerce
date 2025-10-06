using EdaMicroEcommerce.Application.IntegrationEvents;
using MediatR;

namespace EdaMicroEcommerce.Application.Outbox;

public class OutboxIntegrationEvent : IRequest
{
    public EventType Type { get; private set; }
    public DateTime? ProcessedAtUtc { get; private set; }
    public string Payload { get; private set; }
    public int RetryCount { get; private set; }
    public bool IsDeadLetter { get; private set; }
    public DateTime CreatedAtUtc { get; set; }

    protected OutboxIntegrationEvent(EventType type, string payload)
    {
        Type = type;
        ProcessedAtUtc = null;
        CreatedAtUtc = DateTime.UtcNow;
        Payload = payload;
        RetryCount = 0;
    }

    public void SetProcessedAtToNow()
    {
        ProcessedAtUtc = DateTime.UtcNow;
    }

    public void UpdateRetryCount()
    {
        RetryCount += 1;
    }

    public void MarkAsDead()
    {
        IsDeadLetter = true;
    }
    
    private OutboxIntegrationEvent()
    {
    } // Ef
}