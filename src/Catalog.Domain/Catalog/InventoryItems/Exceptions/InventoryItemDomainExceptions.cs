using EdaMicroEcommerce.Domain.BuildingBlocks;

namespace Catalog.Domain.Catalog.InventoryItems.Exceptions;

public class InventoryItemInvalidReservationException(string message) : DomainException(message);