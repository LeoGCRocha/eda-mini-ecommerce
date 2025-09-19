using EdaMicroEcommerce.Domain.BuildingBlocks;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace EdaMicroEcommerce.Domain.Catalog.Products.Events;

public class ProductDeactivationEvent : IDomainEvent
{
    public ProductId ProductId { get; set; }
}