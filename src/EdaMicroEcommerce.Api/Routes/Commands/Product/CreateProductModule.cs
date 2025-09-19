using Carter;
using EdaMicroEcommerce.Application.CQS.Commands.Products;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EdaMicroEcommerce.Api.Routes.Commands.Product;

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