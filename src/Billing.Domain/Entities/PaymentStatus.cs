namespace Billing.Domain.Entities;

public enum PaymentStatus
{
    Created = 1,
    Approved = 2,
    Refused = 3,
    Canceled = 4
}