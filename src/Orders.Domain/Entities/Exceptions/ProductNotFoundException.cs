using EdaMicroEcommerce.Domain.BuildingBlocks;

namespace Orders.Domain.Entities.Exceptions;

public class ProductNotFoundException(string message) : DomainException(message);