using EdaMicroEcommerce.Domain.BuildingBlocks;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;
using EdaMicroEcommerce.Domain.Catalog.Products.Events;
using EdaMicroEcommerce.Domain.Catalog.Products.Exceptions;

namespace EdaMicroEcommerce.Domain.Catalog.Products;

public sealed class Product : AggregateRoot<ProductId>
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal BasePrice { get; private set; }
    public bool IsActive { get; private set; } = true;

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
        IsActive = false;
        AddDomainEvent(new ProductDeactivationEvent());
    }
    
    private Product() {} // Ef
}