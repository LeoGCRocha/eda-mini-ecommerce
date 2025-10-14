using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Orders.Api.CQS.CreateOrder;

public class CreateOrderModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1/order", async (CreateOrderCommand cmd, IMediator mediator) =>
        {
            await mediator.Send(cmd);
            return Results.NoContent();
        });
    }
}