using EdaMicroEcommerce.Domain.BuildingBlocks;
using Catalog.Domain.Entities.InventoryItems.Events;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;
using EdaMicroEcommerce.Domain.Enums;

namespace Catalog.Domain.Entities.InventoryItems;

public sealed class InventoryItem : AggregateRoot<InventoryItemId>
{
    public ProductId ProductId { get; private set; }
    public int AvailableQuantity { get; private set; }
    public int ReservedQuantity { get; private set; }
    public int ReorderLevel { get; private set; }

    private readonly List<Reservation> _reservations = [];
    public IReadOnlyList<Reservation> Reservations => _reservations.AsReadOnly();

    private InventoryItem(List<Reservation> reservations)
    {
        _reservations = reservations;
    } // Ef

    public InventoryItem(ProductId productId, int availableQuantity = 0, int reorderLevel = 0, int reservedQuantity = 0)
    {
        ProductId = productId;
        AvailableQuantity = availableQuantity;
        ReservedQuantity = reservedQuantity;
        ReorderLevel = reorderLevel;
    }

    public bool
        ReserveQuantity(int quantity, OrderId orderId) // <WARNING> Talvez isso aqui não tenha ficado tão bom por que
        // estou "quebrando os boundaries" fazendo ele saber do ID da Order só pra "facilitar meu caminho".
    {
        if (quantity > AvailableQuantity)
        {
            AddDomainEvent(new ProductReservedEvent(ProductId, Id, quantity, orderId, ReservationEventType.Failure));
            return false;
        }

        if (_reservations.FirstOrDefault(or => or.OrderId == orderId && or.Status == ReservationStatus.Reserved) is not
            null)
            throw new GenericException($"Tentativa dupla de reserva do produto para o pedido ({orderId})");

        ReservedQuantity += quantity;
        AvailableQuantity -= quantity;
        
        _reservations.Add(new Reservation(orderId, ReservationStatus.Reserved, quantity));

        AddDomainEvent(new ProductReservedEvent(ProductId, Id, quantity, orderId, ReservationEventType.Reservation));

        if (AvailableQuantity <= ReorderLevel)
            AddDomainEvent(new ProductLowStockEvent(ProductId, Id, ReorderLevel));

        return true;
    }

    public void CancelReservation(OrderId orderId, int quantity)
    {
        if (_reservations.FirstOrDefault(or => or.OrderId == orderId && or.Status == ReservationStatus.Cancelled) is not
            null)
            throw new GenericException($"Tentativa dupla de cancelamento do produto ({ProductId}) para o pedido ({orderId})");
        
        _reservations.Add(new Reservation(orderId, ReservationStatus.Cancelled, quantity));
        
        ReservedQuantity -= quantity;
        AvailableQuantity += quantity;
    }

    public void ConfirmReservation(OrderId orderId, int quantity)
    {
        if (_reservations.FirstOrDefault(or => or.OrderId == orderId && or.Status == ReservationStatus.Confirmed) is not
            null)
            throw new GenericException("The Idempotency error confirmation event was found twice.");
        
        _reservations.Add(new Reservation(orderId, ReservationStatus.Confirmed, quantity));

        ReservedQuantity -= quantity;
        AvailableQuantity += quantity;
    }

    public void MakeUnavailable()
    {
        // <WARNING> Em uma estrutura mais robusta real seria necessário pensar uma forma de como lidar com os produtos
        // já reservados. Na visão de produto definida aqui, o Available é definido como zero para impedir novas compras
        // e o que já foi reservado permanecerá igual.
        AvailableQuantity = 0;
    }
}