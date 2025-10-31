using EdaMicroEcommerce.Domain.BuildingBlocks;

namespace Orders.Domain.Entities.Exceptions;

public class InvalidAmountException(string message) : DomainException(message);