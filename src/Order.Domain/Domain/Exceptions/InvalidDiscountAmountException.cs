using EdaMicroEcommerce.Domain.BuildingBlocks;

namespace Order.Domain.Domain.Exceptions;

public class InvalidDiscountAmountException(string message): DomainException(message);