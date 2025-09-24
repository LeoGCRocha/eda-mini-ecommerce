using EdaMicroEcommerce.Application.IntegrationEvents.Products;
using EdaMicroEcommerce.Application.Outbox;
using EdaMicroEcommerce.Infra.MessageBroker.Builders;
using EdaMicroEcommerce.Infra.MessageBroker.Products;

namespace EdaMicroEcommerce.Infra.MessageBroker.ProducerBuilder;

public static class ProducerBaseFactory
{
    public static ProducerBase GetProducerNameFromEvent(OutboxIntegrationEvent evt)
    {
        switch (evt.Type)
        {
            case nameof(ProductDeactivationIntegration):
                return new ProductDeactivated();
            default:
                throw new Exception("Invalid type event.");
        }
    }
}