namespace Orders.Application.IntegrationEvents;

public enum EventType
{
    // <ORDER CREATED>
    OrderCreated = 1,
    // <PRODUCT RESERVATION>
    ProductReservation = 2,
    // <PAYMENT PEDING>
    PaymentPending = 3
}