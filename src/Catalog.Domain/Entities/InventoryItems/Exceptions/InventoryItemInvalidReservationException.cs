using EdaMicroEcommerce.Domain.BuildingBlocks;

namespace Catalog.Domain.Entities.InventoryItems.Exceptions;

public class InventoryItemInvalidReservationException(string message) : DomainException(message);