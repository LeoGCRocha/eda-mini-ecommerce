using EdaMicroEcommerce.Domain.BuildingBlocks;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace EdaMicroEcommerce.Domain.Catalog.Products.Events;

public class ProductDeactivatedEvent : IDomainEvent
{
    public ProductId ProductId { get; set; }

    public ProductDeactivatedEvent(ProductId productId)
    {
        ProductId = productId;
    }
}