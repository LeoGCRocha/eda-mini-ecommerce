using Carter;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Billing.Api.CQS;

public class ProcessPaymentModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/v1/payment/{id}/process", async (Guid id, ProcessPaymentCommand cmd, IMediator mediator) =>
            {
                cmd.PaymentId = new PaymentId(id);
                var results = await mediator.Send(cmd);
                return Results.Ok(results);
            })
        .WithTags("Payment");
    }
}