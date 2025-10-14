namespace Orders.Domain.Entities;

public enum OrderStatus
{
    Draft = 0,
    Created = 1,
    ReservationPending = 2,
    ReservationFailed = 3,
    PaymentPending = 4,
    Paid = 5,
    Canceled = 6,
}