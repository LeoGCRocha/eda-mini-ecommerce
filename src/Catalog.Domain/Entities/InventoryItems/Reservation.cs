using EdaMicroEcommerce.Domain.Enums;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Catalog.Domain.Entities.InventoryItems;

public class Reservation
{
    public OrderId OrderId { get; set; }
    public ReservationStatus Status { get; set; }
    public int Quantity { get; set; }
    public DateTime OccuredAtUtc { get; set; }

    public Reservation(OrderId orderId, ReservationStatus status, int quantity, DateTime? occuredAtUtc = null)
    {
        OrderId = orderId;
        Status = status;
        Quantity = quantity;
        OccuredAtUtc = occuredAtUtc ?? DateTime.UtcNow;
    }

    private Reservation()
    {
    } // Ef
}