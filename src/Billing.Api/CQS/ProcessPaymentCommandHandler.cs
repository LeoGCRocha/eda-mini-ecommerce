using MediatR;
using Billing.Application.Services;

namespace Billing.Api.CQS;

public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, ProcessPaymentResponse>
{
    private readonly IPaymentCouponService _paymentCouponService;

    public ProcessPaymentCommandHandler(IPaymentCouponService paymentCouponService)
    {
        _paymentCouponService = paymentCouponService;
    }

    public async Task<ProcessPaymentResponse> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _paymentCouponService.ProcessPayment(request.PaymentId, request.CouponName);

        return new ProcessPaymentResponse()
        {
            PaymentId = payment.Id,
            PaymentStatus = payment.Status
        };
    }
}