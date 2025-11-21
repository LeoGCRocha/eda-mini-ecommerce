using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;
using EdaMicroEcommerce.Domain.Enums;

namespace Orders.Domain.Entities.Events;

public record ProductReservationStatusEvent(
    ProductId ProductId,
    InventoryItemId InventoryItemId,
    OrderId OrderId,
    int ReservedQuantity,
    ReservationEventType ReservationEventType);