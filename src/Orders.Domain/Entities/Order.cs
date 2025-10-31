using EdaMicroEcommerce.Domain.BuildingBlocks;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;
using EdaMicroEcommerce.Domain.Enums;
using Orders.Domain.Entities.Events;
using Orders.Domain.Entities.Exceptions;

namespace Orders.Domain.Entities;

public class Order : AggregateRoot<OrderId>
{
    public CustomerId CustomerId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string Currency { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal NetAmount { get; private set; }
    public OrderStatus Status { get; private set; }
    public PaymentId?  PaymentId { get; private set; }
    public DateTime? PaymentDate { get; private set; }
    private readonly List<OrderItem> _orderItems = [];
    
    public IReadOnlyList<OrderItem> OrderItems => _orderItems.AsReadOnly();

    private Order()
    {
    } // Ef

    public Order(CustomerId customerId, List<OrderItem> orderItems, decimal discountAmount = 0.0m,
        string currency = "BRL")
    {
        CustomerId = customerId;
        Currency = currency;
        foreach (var orderItem in orderItems)
            AddOrderItem(orderItem);
        ApplyDiscount(discountAmount);
        Status = OrderStatus.Created;
        AddDomainEvent(new OrderCreatedEvent(Id,
            _orderItems.Select(oe => new ProductOrderInfo(oe.ProductId, oe.Quantity)).ToList()));
    }

    private void AddOrderItem(OrderItem orderItem)
    {
        var firstOrDefault = _orderItems.FirstOrDefault(currOrderItem => currOrderItem.Id == orderItem.Id);
        if (firstOrDefault is not null)
            throw new TryAddingOrderItemException("Um item não pode ser adicionado duas vezes a um pedido.");
        _orderItems.Add(orderItem);
        TotalAmount += orderItem.Total();
    }

    private void ApplyDiscount(decimal discountAmount)
    {
        if (discountAmount >= TotalAmount)
            throw new InvalidDiscountAmountException("Desconto não pode ser maior que o preço total do produto");
        NetAmount = TotalAmount - discountAmount;
        DiscountAmount = discountAmount;
    }
    
    public void CancelOrder(string reason)
    {
        ChangeStatus(OrderStatus.Canceled);
        AddDomainEvent(new OrderCanceledEvent(Id, reason));
    }

    public void ChangeStatus(OrderStatus newStatus)
    {
        Status = Status switch
        {
            OrderStatus.Canceled => throw new InvalidStatusChangeException(
                "Pedido não pode mudar de status apos cancelado."),
            OrderStatus.Paid => throw new InvalidStatusChangeException(
                "Pedido não pode mudar de estado apos confirmado."),
            _ => newStatus
        };
    }
    
    public List<(ProductId, int)> UpdateOrderItensStatus(List<ProductId> productsIds, ReservationStatus status)
    {
        var productsWithQuantity = new List<(ProductId, int)>();
        
        var orderItems = _orderItems.Where(or => productsIds.Contains(or.ProductId));
        
        foreach (var orderItem in orderItems) {
            orderItem.ReservationStatus = status;
            productsWithQuantity.Add(new ValueTuple<ProductId, int>(orderItem.ProductId, orderItem.Quantity));
        }

        return productsWithQuantity;
    }
}