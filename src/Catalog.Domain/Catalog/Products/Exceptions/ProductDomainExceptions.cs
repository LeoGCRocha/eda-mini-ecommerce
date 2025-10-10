using EdaMicroEcommerce.Domain.BuildingBlocks;

namespace Catalog.Domain.Catalog.Products.Exceptions;

// Invariants
public class ProductDomainExceptions(string message) : DomainException(message);