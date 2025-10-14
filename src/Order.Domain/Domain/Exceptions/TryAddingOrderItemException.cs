using EdaMicroEcommerce.Domain.BuildingBlocks;

namespace Order.Domain.Domain.Exceptions;

public class TryAddingOrderItemException(string message) : DomainException(message);