using EdaMicroEcommerce.Infra.MessageBroker.Builders;

namespace EdaMicroEcommerce.Infra.MessageBroker.Products;

public class ProductDeactivated() : ProducerBase(TopicName)
{
    // const always works as static
    // this news indicates that we want to hide de base TopicName
    // TODO: this should be on a configuration instead of here
    public const string TopicName = "product-deactivated"; // bad implementation
}

public class ProductDeactivatedConsumer() : ConsumerBase(TopicName)
{
    public const string TopicName = "product-deactivated";
}