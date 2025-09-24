using Carter;
using MediatR;
using EdaMicroEcommerce.Application.CQS.Commands.Products;

namespace EdaMicroEcommerce.Api.Routes.Commands.Product;

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