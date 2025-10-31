using System.Text.Json;
using Catalog.Application.IntegrationEvents.Products.ProductDeactivated;
using Catalog.Domain.Entities.InventoryItems.Events;
using Catalog.Domain.Entities.Products.Events;

namespace Catalog.Application.IntegrationEvents.Products;

public static class ProductIntegrationFactory
{
    public static ProductDeactivatedIntegration FromDomain(ProductDeactivatedEvent evt)
    {
        var payload = JsonSerializer.Serialize(evt);
        return new ProductDeactivatedIntegration(EventType.ProductDeactivated, payload);
    }

    public static ProductReservedIntegration FromDomain(ProductReservedEvent evt)
    {
        var payload = JsonSerializer.Serialize(evt);
        return new ProductReservedIntegration(EventType.ProductReserved, payload);
    }
}