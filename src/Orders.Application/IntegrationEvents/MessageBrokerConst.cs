namespace Orders.Application.IntegrationEvents;

public static class MessageBrokerConst
{
    public const string OrderCreatedProducer = "OrderCreatedProducer";
    public const string OrderCreatedConsumer = "OrderCreatedConsumer";
    public const string ProductReservationProducer = "ProductReservationProducer";
    public const string ProductReservedConsumer = "ProductReservedConsumer";
    public const string PaymentPendingProducer = "PaymentPendingProducer";
}