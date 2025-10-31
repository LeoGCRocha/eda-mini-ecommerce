using EdaMicroEcommerce.Domain.BuildingBlocks;

namespace Orders.Domain.Entities.Exceptions;

public class InvalidStatusChangeException(string message) : DomainException(message);