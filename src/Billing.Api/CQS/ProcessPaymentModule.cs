using Carter;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Billing.Api.CQS;

public class ProcessPaymentModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/v1/payment/{id}/process", async (Guid id, [FromBody] RequestParams @params, [FromServices] IMediator mediator) =>
            {
                var results = await mediator.Send(new ProcessPaymentCommand()
                {
                    PaymentId = new PaymentId(id),
                    CouponName = @params.CouponName
                });
                return Results.Ok(results);
            })
        .WithTags("Payment");
    }

    public class RequestParams
    {
        public string? CouponName { get; set; }
    }
}