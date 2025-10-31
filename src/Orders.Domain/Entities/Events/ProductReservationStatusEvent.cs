using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;
using EdaMicroEcommerce.Domain.Enums;

namespace Orders.Domain.Entities.Events;

// TODO: COMO CENTRALIZR ESSAS PARTES COMUMMM ?
// TODO: COMO SCHEMA REGISTRY LIDA COM VERSIONAMENTO.....
public record ProductReservationStatusEvent(
    ProductId ProductId,
    InventoryItemId InventoryItemId,
    OrderId OrderId,
    int ReservedQuantity,
    ReservationEventType ReservationEventType);