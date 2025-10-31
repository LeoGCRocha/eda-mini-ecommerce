using MediatR;
using System.Text.Json;
using EdaMicroEcommerce.Application.Outbox;
using Platform.SharedContracts.IntegrationEvents.Products;

namespace Orders.Application.IntegrationEvents.Products;

public class ProductReservationIntegrationHandler : IRequestHandler<ProductReservationIntegration>
{
    private readonly IIntegrationEventPublisher _eventPublisher;

    public ProductReservationIntegrationHandler(IIntegrationEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(ProductReservationIntegration request, CancellationToken cancellationToken)
    {
        var @object = JsonSerializer.Deserialize<ProductReservationEvent>(request.Payload);
        // <WARNING> Usando PRODUCT ID como key assim todos produtos iguais vão para mesma partição e portanto
        // respeitam uma ordem.
        // Esta é a primeira etapa para o controle de concorrência em um inventario para garantir que diferentes consumidores (exemplo replicas dessa app)
        // Não recebam produtos de mesmo id oque traria condição de corrida para banco de dados.
        await _eventPublisher.PublishOnTopicAsync(@object, MessageBrokerConst.ProductReservationProducer, @object!.ProductId.Value.ToString());
    }
}