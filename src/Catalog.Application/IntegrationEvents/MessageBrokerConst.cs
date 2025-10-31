namespace Catalog.Application.IntegrationEvents;

public static class MessageBrokerConst
{
    // TODO: Mudar isso pra vir do arquivo
    // <WARNING> Esse nome precisa estar igual ao que vem nos arquivos de configs, talvez seria melhor uma maneira mais robusta pra isso
    public const string ProductDeactivatedProducer = "ProductDeactivatedProducer";
    public const string ProductDeactivatedConsumer = "ProductDeactivatedConsumer";
    public const string InventoryReservationConsumer = "InventoryReservationConsumer";
    public const string ProductReservedProducer = "ProductReservedProducer";
}