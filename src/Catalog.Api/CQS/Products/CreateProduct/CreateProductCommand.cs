using MediatR;

namespace Catalog.Api.CQS.Products.CreateProduct;

public class CreateProductCommand : IRequest
{
    public string Name { get; set; }
    public string Description  { get; set; }
    public decimal Price { get; set; }
    public int AvailableQuantity { get; set; }
    public int ReorderQuantity { get; set; }
}
