using EdaMicroEcommerce.Domain.Catalog;
using MediatR;

namespace EdaMicroEcommerce.Api.Features.Commands.Products.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand>
{
    private readonly IProductInventoryService _productInventoryService;

    public CreateProductCommandHandler(IProductInventoryService productInventoryService)
    {
        _productInventoryService = productInventoryService;
    }

    public async Task Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Domain.Catalog.Products.Product(request.Name, request.Description, request.Price);

        await _productInventoryService.CreateProductAndInventoryAsync(product, request.AvailableQuantity,
            request.ReorderQuantity);
    }
}