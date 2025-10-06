using Carter;
using MediatR;

namespace EdaMicroEcommerce.Api.Features.Commands.Products.DeactivateProduct;

public class DeactivateProductModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // TODO: VER SOBRE SCHEMA REGISTRY
        app.MapPost("api/v1/product/deactivate", async (DeactivateProductCommand cmd, IMediator mediator) =>
            {
                await mediator.Send(cmd);
                return Results.NoContent();
            })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}