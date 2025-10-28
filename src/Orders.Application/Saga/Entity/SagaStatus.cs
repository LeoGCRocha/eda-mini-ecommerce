namespace Orders.Application.Saga.Entity;

public enum SagaStatus
{
    OrderCreated,
    PendingReservation,
    PendingPayment,
    FailedReservation,
    Finished,
    Failed
}