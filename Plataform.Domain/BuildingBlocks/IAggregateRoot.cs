using System.Collections.ObjectModel;

namespace EdaMicroEcommerce.Domain.BuildingBlocks;

public interface IAggregateRoot
{
    ReadOnlyCollection<IDomainEvent> GetDomainEvents();
    void ClearDomainEvents();
}