namespace Orders.Domain.Entities;

public enum OrderStatus
{
    Draft,
    Created,
    PendingReservation,
    FailedReservation,
    PendingPayment,
    Paid,
    Canceled
}