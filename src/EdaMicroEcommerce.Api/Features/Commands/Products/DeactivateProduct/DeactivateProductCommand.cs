using MediatR;

namespace EdaMicroEcommerce.Api.Features.Commands.Products.DeactivateProduct;

public class DeactivateProductCommand : IRequest
{
    public Guid ProductId { get; set; }
}

