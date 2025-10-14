namespace Catalog.Application.IntegrationEvents;

public static class MessageBrokerConst
{
    // <WARNING> Esse nome precisa estar igual ao que vem nos arquivos de configs, talvez seria melhor uma maneira mais robusta pra isso
    public const string ProductDeactivatedProducer = "ProductDeactivatedProducer";
    public const string ProductDeactivatedCosnumer = "ProductDeactivatedConsumer";
}