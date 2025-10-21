namespace Orders.Application.Saga.Entity;

public enum SagaStatus
{
    ORDER_CREATED,
    PENDING_RESERVATION,
    PENDING_PAYMENT,
    FAILED_RESERVATION,
    FINISHED,
    FAILED
}