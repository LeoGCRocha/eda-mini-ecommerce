using System.Diagnostics;
using Billing.Application.Observability;
using KafkaFlow;
using Billing.Domain.Entities;
using Microsoft.Extensions.Logging;
using Billing.Application.Repositories;
using Platform.SharedContracts.IntegrationEvents.Payments;

namespace Billing.PaymentWorker.IntegrationEvents;

public class PaymentPendingMessageHandler(
    ILogger<PaymentPendingMessageHandler> logger,
    IPaymentRepository paymentRepository)
    : IMessageHandler<PaymentPendingEvent>
{
    public async Task Handle(IMessageContext context, PaymentPendingEvent message)
    {
        using var activity = Source.BillingSource.StartActivity("Creating new pending payment", ActivityKind.Server);
        activity?.SetTag("order.id", message.OrderId.Value);
        activity?.SetTag("customer.id", message.CustomerId.Value);

        try
        {
            logger.LogInformation("Starting to create a new payment from an order creation.");

            var payment = await paymentRepository.GetPaymentFromOrderIdAsync(message.OrderId);

            if (payment is not null)
                throw new Exception("Payment creation was attempted twice.");

            await paymentRepository.AddPaymentAsync(new Payment(message.TotalAmount, message.OrderId, message.CustomerId));

            logger.LogInformation("Payment was successfully created.");

            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create a new payment.");

            activity?.SetStatus(ActivityStatusCode.Error);
        }
    }
}