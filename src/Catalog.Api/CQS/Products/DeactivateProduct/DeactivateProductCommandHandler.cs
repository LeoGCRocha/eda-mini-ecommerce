using Catalog.Domain.Catalog;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;
using MediatR;

namespace Catalog.Api.CQS.Products.DeactivateProduct;

public class DeactivateProductCommandHandler(IProductInventoryService productRepository)
    : IRequestHandler<DeactivateProductCommand>
{
    public async Task Handle(DeactivateProductCommand request, CancellationToken cancellationToken)
    {
        await productRepository.DeactivateProductAsync(new ProductId(request.ProductId));
    }
}