using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Orders.Domain.Entities.Events;

// TODO: COMO CENTRALIZR ESSAS PARTES COMUMMM ?
// TODO: COMO SCHEMA REGISTRY LIDA COM VERSIONAMENTO.....
public class ProductReservedEvent
{
    public ProductId ProductId { get; set; }
    public InventoryItemId InventoryItemId { get; set; }
    public OrderId OrderId { get; set; }
    public int ReservedQuantity { get; set; }
    public bool IsReservationSucceed { get; set; }
}