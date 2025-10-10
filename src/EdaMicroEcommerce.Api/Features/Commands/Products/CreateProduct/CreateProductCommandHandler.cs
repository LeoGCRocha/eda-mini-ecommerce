using MediatR;
using Catalog.Domain.Catalog;
using Catalog.Domain.Catalog.Products;

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
        var product = new Product(request.Name, request.Description, request.Price);

        await _productInventoryService.CreateProductAndInventoryAsync(product, request.AvailableQuantity,
            request.ReorderQuantity);
    }
}