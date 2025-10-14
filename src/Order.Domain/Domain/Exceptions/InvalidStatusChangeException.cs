using EdaMicroEcommerce.Domain.BuildingBlocks;

namespace Order.Domain.Domain.Exceptions;

public class InvalidStatusChangeException(string message) : DomainException(message);