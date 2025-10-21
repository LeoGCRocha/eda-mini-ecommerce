namespace Orders.Domain.Entities;

public enum OrderStatus
{
    DRAFT = 0,
    CREATED = 1,
    PENDING_RESERVATION = 2,
    FAILED_RESERVATION = 3,
    PENDING_PAYMENT = 4,
    PAID = 5,
    CANCELED = 6,
}