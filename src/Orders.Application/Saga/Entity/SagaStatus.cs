namespace Orders.Application.Saga.Entity;

public enum SagaStatus
{
    ORDER_CREATED,
    PENDING_RESERVATION,
    PENDING_PAYMENT,
    COMPENSATE_RESERVATION,
    FINISHED,
    FAILED
}