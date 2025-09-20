using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;
using EdaMicroEcommerce.Domain.Catalog;
using MediatR;

namespace EdaMicroEcommerce.Application.CQS.Commands.Products;

public class DeactivateProductCommand : IRequest
{
    public Guid ProductId { get; set; }
}

public class DeactivateProductCommandHandler : IRequestHandler<DeactivateProductCommand>
{
    private readonly IProductInventoryService _productInventoryService;
    
    public DeactivateProductCommandHandler(IProductInventoryService productRepository)
    {
        _productInventoryService = productRepository;
    }

    public async Task Handle(DeactivateProductCommand request, CancellationToken cancellationToken)
    {
        await _productInventoryService.DeactivateProduct(new ProductId(request.ProductId));
    }
}