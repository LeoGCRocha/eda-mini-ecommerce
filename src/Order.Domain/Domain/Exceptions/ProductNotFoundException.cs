using EdaMicroEcommerce.Domain.BuildingBlocks;

namespace Order.Domain.Domain.Exceptions;

public class ProductNotFoundException(string message) : DomainException(message);