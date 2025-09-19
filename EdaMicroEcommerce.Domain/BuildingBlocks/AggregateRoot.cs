namespace EdaMicroEcommerce.Domain.BuildingBlocks;

public abstract class AggregateRoot<T> : Entity<T> where T : new()
{
    public AggregateRoot() {}
    
    public IReadOnlyList<IDomainEvent> GetDomainEvents => _domainEvents.AsReadOnly();

    public void ClearDomainEvents()
    {
        _domainEvents = [];
    }

    public void AddDomainEvent(IDomainEvent domainEvent) =>
        _domainEvents.Add(domainEvent);

    public void AddDomainEvents(List<IDomainEvent> domainEvents) =>
        _domainEvents.AddRange(domainEvents);
    
    private List<IDomainEvent> _domainEvents = [];
}