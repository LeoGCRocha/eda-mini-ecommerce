using Catalog.Application.Observability;
using Catalog.Domain.Entities;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;
using MediatR;

namespace Catalog.Api.CQS.Products.DeactivateProduct;

public class DeactivateProductCommandHandler(IProductInventoryService productRepository)
    : IRequestHandler<DeactivateProductCommand>
{
    public async Task Handle(DeactivateProductCommand request, CancellationToken cancellationToken)
    {
        using var activity =
            Source.CatalogSource.StartActivity($"{nameof(DeactivateProductCommandHandler)} : Deactivating product");

        activity?.SetTag("product.id", request.ProductId);
        
        await productRepository.DeactivateProductAsync(new ProductId(request.ProductId));
    }
}