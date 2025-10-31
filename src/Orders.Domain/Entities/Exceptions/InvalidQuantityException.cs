using EdaMicroEcommerce.Domain.BuildingBlocks;

namespace Orders.Domain.Entities.Exceptions;

public class InvalidQuantityException(string message) : DomainException(message);