using EdaMicroEcommerce.Domain.BuildingBlocks;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;
using EdaMicroEcommerce.Domain.Catalog.Products.Events;
using EdaMicroEcommerce.Domain.Catalog.Products.Exceptions;

namespace EdaMicroEcommerce.Domain.Catalog.Products;

public sealed class Product : AggregateRoot<ProductId>
{
    // TODO: nunca confiar no valor em memoria ver sobre ATOMIC UPDATE e OPTIMISTIC LOCKING
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal BasePrice { get; private set; }
    public bool IsActive { get; private set; } = true;
    
    // TODO: No saga seria bom confirmar que o produto esta ativo antes de confirmar a order;
    // TODO: Talvez fazer OPTIMISTIC LOCKING

    public Product(string name, string description, decimal basePrice, bool isActive = true)
    {
        Name = name;
        Description = description;
        BasePrice = basePrice;
        IsActive = isActive;

        if (BasePrice < 0)
            throw new ProductDomainExceptions("Product cannot have price lower than zero.");
    }

    public void DeactivateProduct()
    {
        if (!IsActive)
            return;
        
        IsActive = false;
        AddDomainEvent(new ProductDeactivatedEvent(Id));
    }
    
    private Product() {} // Ef
}