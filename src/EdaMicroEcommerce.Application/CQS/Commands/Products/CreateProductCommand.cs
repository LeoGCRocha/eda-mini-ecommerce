using EdaMicroEcommerce.Domain.Catalog;
using EdaMicroEcommerce.Domain.Catalog.Products;
using MediatR;

namespace EdaMicroEcommerce.Application.CQS.Commands.Products;

public class CreateProductCommand : IRequest
{
    public string Name { get; set; }
    public string Description  { get; set; }
    public decimal Price { get; set; }
    public int AvailableQuantity { get; set; }
    public int ReorderQuantity { get; set; }
}

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