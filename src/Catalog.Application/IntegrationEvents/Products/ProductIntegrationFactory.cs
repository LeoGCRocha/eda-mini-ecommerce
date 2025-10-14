using System.Text.Json;
using Catalog.Domain.Catalog.Products.Events;
using Catalog.Application.IntegrationEvents.Products.ProductDeactivated;

namespace Catalog.Application.IntegrationEvents.Products;

public static class ProductIntegrationFactory
{
    public static ProductDeactivatedIntegration FromDomain(ProductDeactivatedEvent evt)
    {
        var payload = JsonSerializer.Serialize(evt);
        return new ProductDeactivatedIntegration(EventType.ProductDeactivated, payload);
    }
}