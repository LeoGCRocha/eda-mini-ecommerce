namespace Catalog.Application.IntegrationEvents;

public static class MessageBrokerConst
{
    // <WARNING> This wasn't the best approach.
    public const string ProductDeactivatedProducer = "ProductDeactivatedProducer";
    public const string ProductDeactivatedConsumer = "ProductDeactivatedConsumer";
    public const string InventoryReservationConsumer = "InventoryReservationConsumer";
    public const string ProductReservedProducer = "ProductReservedProducer";
}