using Catalog.Domain.Catalog;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;
using MediatR;

namespace EdaMicroEcommerce.Api.Features.Commands.Products.DeactivateProduct;

public class DeactivateProductCommandHandler : IRequestHandler<DeactivateProductCommand>
{
    private readonly IProductInventoryService _productInventoryService;
    
    public DeactivateProductCommandHandler(IProductInventoryService productRepository)
    {
        _productInventoryService = productRepository;
    }

    public async Task Handle(DeactivateProductCommand request, CancellationToken cancellationToken)
    {
        await _productInventoryService.DeactivateProductAsync(new ProductId(request.ProductId));
    }
}