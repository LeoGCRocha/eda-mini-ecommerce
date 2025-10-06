using System.Text.Json;
using EdaMicroEcommerce.Application.IntegrationEvents.Products.ProductDeactivated;
using EdaMicroEcommerce.Domain.Catalog.Products.Events;

namespace EdaMicroEcommerce.Application.IntegrationEvents.Products;

public static class ProductIntegrationFactory
{
    public static ProductDeactivatedIntegration FromDomain(ProductDeactivatedEvent evt)
    {
        var payload = JsonSerializer.Serialize(evt);
        return new ProductDeactivatedIntegration(EventType.ProductDeactivated, payload);
    }
}