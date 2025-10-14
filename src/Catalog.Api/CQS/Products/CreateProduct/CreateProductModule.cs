using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Catalog.Api.CQS.Products.CreateProduct;

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