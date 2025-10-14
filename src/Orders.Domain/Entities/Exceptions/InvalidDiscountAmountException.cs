using EdaMicroEcommerce.Domain.BuildingBlocks;

namespace Orders.Domain.Entities.Exceptions;

public class InvalidDiscountAmountException(string message): DomainException(message);