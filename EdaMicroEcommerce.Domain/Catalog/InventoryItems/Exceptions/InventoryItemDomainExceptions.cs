using EdaMicroEcommerce.Domain.BuildingBlocks;

namespace EdaMicroEcommerce.Domain.Catalog.InventoryItems.Exceptions;

public class InventoryItemInvalidReservationException(string message) : DomainException(message);