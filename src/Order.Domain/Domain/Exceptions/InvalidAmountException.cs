using EdaMicroEcommerce.Domain.BuildingBlocks;

namespace Order.Domain.Domain.Exceptions;

public class InvalidAmountException(string message) : DomainException(message);