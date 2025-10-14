using EdaMicroEcommerce.Domain.BuildingBlocks;

namespace Catalog.Domain.Entities.Products.Exceptions;

// Invariants
public class ProductDomainExceptions(string message) : DomainException(message);