using EdaMicroEcommerce.Domain.BuildingBlocks;
using Catalog.Domain.Entities.InventoryItems.Events;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Catalog.Domain.Entities.InventoryItems;

public sealed class InventoryItem : AggregateRoot<InventoryItemId>
{
    public ProductId ProductId { get; private set; }
    public int AvailableQuantity { get; private set; }
    public int ReservedQuantity { get; private set; }
    public int ReorderLevel { get; private set; }

    private InventoryItem() {} // Ef
    
    public InventoryItem(ProductId productId, int availableQuantity = 0, int reorderLevel = 0, int reservedQuantity = 0)
    {
        ProductId = productId;
        AvailableQuantity = availableQuantity;
        ReservedQuantity = reservedQuantity;
        ReorderLevel = reorderLevel;
    }

    public bool ReserveQuantity(int quantity, OrderId orderId) // <WARNING> Talvez isso aqui não tenha ficado tão bom por que
    // estou "quebrando os boundaries" fazendo ele saber do ID da Order só pra "facilitar meu caminho".
    {
        if (quantity > AvailableQuantity)
        {
            AddDomainEvent(new ProductReservedEvent(ProductId, Id, quantity, orderId, false));
            return false;
        }
            
        ReservedQuantity += quantity;
        AvailableQuantity -= quantity;

        AddDomainEvent(new ProductReservedEvent(ProductId, Id, quantity, orderId, true));
        if (AvailableQuantity <= ReorderLevel)
            AddDomainEvent(new ProductLowStockEvent(ProductId, Id, ReorderLevel));

        return true;
    }

    public void CancelReservation(int quantity)
    {
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