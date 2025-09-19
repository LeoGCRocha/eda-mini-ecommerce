using EdaMicroEcommerce.Domain.BuildingBlocks;

namespace EdaMicroEcommerce.Domain.Catalog.Products.Exceptions;

// Invariants
public class ProductDomainExceptions(string message) : DomainException(message);