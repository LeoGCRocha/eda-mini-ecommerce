using EdaMicroEcommerce.Domain.BuildingBlocks;

namespace Order.Domain.Domain.Exceptions;

public class TryAddingOrderItemException(string message) : DomainException(message);
public class InvalidDiscountAmountException(string message): DomainException(message);
public class ProductNotFoundException(string message) : DomainException(message);
public class InvalidAmountException(string message) : DomainException(message);
public class InvalidStatusChangeException(string message) : DomainException(message);