namespace EdaMicroEcommerce.Application.Outbox;

public class OutboxIntegrationEvent
{
    public string Type { get; private set; }
    public DateTime? ProcessedAtUtc { get; private set; }
    public string Payload { get; private set; }
    public int RetryCount { get; private set; }
    public DateTime CreatedAtUtc { get; set; }

    protected OutboxIntegrationEvent(string type, string payload)
    {
        Type = type;
        ProcessedAtUtc = null;
        CreatedAtUtc = DateTime.UtcNow;
        Payload = payload;
        RetryCount = 0;
    }

    private OutboxIntegrationEvent()
    {
    } // Ef
}