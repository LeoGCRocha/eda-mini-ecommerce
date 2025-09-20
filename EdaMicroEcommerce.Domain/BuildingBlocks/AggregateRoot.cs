using System.Collections.ObjectModel;

namespace EdaMicroEcommerce.Domain.BuildingBlocks;

public abstract class AggregateRoot<T> : Entity<T>, IAggregateRoot where T : new()
{
    public AggregateRoot() {}
    
    public void ClearDomainEvents()
    {
        _domainEvents = [];
    }

    public void AddDomainEvent(IDomainEvent domainEvent) =>
        _domainEvents.Add(domainEvent);

    public void AddDomainEvents(List<IDomainEvent> domainEvents) =>
        _domainEvents.AddRange(domainEvents);
    
    private List<IDomainEvent> _domainEvents = [];
    
    public ReadOnlyCollection<IDomainEvent> GetDomainEvents()
    {
        return _domainEvents.AsReadOnly();
    }
}