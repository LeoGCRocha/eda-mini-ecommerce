using EdaMicroEcommerce.Application.IntegrationEvents;
using EdaMicroEcommerce.Application.IntegrationEvents.Products;
using EdaMicroEcommerce.Application.Outbox;
using EdaMicroEcommerce.Domain.BuildingBlocks;
using EdaMicroEcommerce.Domain.Catalog.Products.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EdaMicroEcommerce.Infra.Persistence;

public class DomainEventInterceptor : SaveChangesInterceptor
{
    // Delegate para retornar a função correta de factory
    // Entender melhor sempre situações que é possivel aplicar
    private readonly Dictionary<Type, Func<IDomainEvent, OutboxIntegrationEvent>> _factoryDictionary = new()
    {
        { typeof(ProductDeactivatedEvent), e => ProductIntegrationFactory.FromDomain((ProductDeactivatedEvent) e) } 
    };

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        return SavingChangesAsync(eventData, result).GetAwaiter().GetResult();
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var context = eventData.Context;

        if (context is null)
            throw new ArgumentException("Expected to receive a context here.");

        var outbox = context.Set<OutboxIntegrationEvent>();
        
        var entries = context!.ChangeTracker.Entries()
            .Where(evt => evt.State is EntityState.Added or EntityState.Modified)
            .Select(e => e.Entity);

        var entriesAggregate = entries.OfType<IAggregateRoot>().ToList();
        var domainEvents = entriesAggregate.SelectMany(e => e.GetDomainEvents());

        foreach (var domainEvt in domainEvents)
        {
            if (_factoryDictionary.TryGetValue(domainEvt.GetType(), out var factoryFunc))
                outbox.Add(factoryFunc(domainEvt));
        }

        foreach (var entry in entriesAggregate)
            entry.ClearDomainEvents();
        
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}