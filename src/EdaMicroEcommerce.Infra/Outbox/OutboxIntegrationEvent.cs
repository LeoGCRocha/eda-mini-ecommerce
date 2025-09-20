namespace EdaMicroEcommerce.Infra.Outbox;

public class OutboxIntegrationEvent
{
    public string Type { get; private set; }
    public DateTime ProcessedAtUtc { get; private set; }
    public string Payload { get; private set; }
    public int RetryCount { get; private set; }

    public OutboxIntegrationEvent(string type, DateTime processedAtUtc, string payload, int retryCount = 0)
    {
        Type = type;
        ProcessedAtUtc = processedAtUtc;
        Payload = payload;
        RetryCount = retryCount;
    }

    private OutboxIntegrationEvent() {} // Ef
}