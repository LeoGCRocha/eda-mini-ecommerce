namespace Billing.Application;

public static class MessageBrokerConst
{
    public const string PaymentPendingConsumer = "PaymentPendingConsumer";
    public const string PaymentProcessedProducer = "PaymentProcessedProducer";
}