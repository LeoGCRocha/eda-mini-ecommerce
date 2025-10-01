using Carter;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EdaMicroEcommerce.Api.Features.Commands.Products.CreateProduct;

public class CreateProductModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1/product", async (CreateProductCommand cmd, IMediator mediator) =>
            {
                await mediator.Send(cmd);
                return Results.Ok();
            })
            .Produces<Created>()
            .WithTags("Product");
    }
}