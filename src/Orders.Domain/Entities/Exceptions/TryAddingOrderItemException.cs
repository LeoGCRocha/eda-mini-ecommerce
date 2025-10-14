using EdaMicroEcommerce.Domain.BuildingBlocks;

namespace Orders.Domain.Entities.Exceptions;

public class TryAddingOrderItemException(string message) : DomainException(message);