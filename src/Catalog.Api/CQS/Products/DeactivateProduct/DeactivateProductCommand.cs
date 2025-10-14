using MediatR;

namespace Catalog.Api.CQS.Products.DeactivateProduct;

public class DeactivateProductCommand : IRequest
{
    public Guid ProductId { get; set; }
}

