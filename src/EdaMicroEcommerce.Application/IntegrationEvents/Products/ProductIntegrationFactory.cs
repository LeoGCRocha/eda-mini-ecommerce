using System.Text.Json;
using EdaMicroEcommerce.Domain.Catalog.Products.Events;

namespace EdaMicroEcommerce.Application.IntegrationEvents.Products;

public static class ProductIntegrationFactory
{
    public static ProductDeactivationIntegration FromDomain(ProductDeactivationEvent evt)
    {
        // Um ponto de melhoria aqui é pensar maneira de lidar com diferentes versões do mesmo payload para evitar possiveis quebras no sistema
        // pensar como seria possuivel evoluir caso tivesse uma variação de uma nova versão do mesmo evento
        var payload = JsonSerializer.Serialize(evt);
        return new ProductDeactivationIntegration(nameof(ProductDeactivationIntegration), payload);
    }
}