using System.Diagnostics;
using Catalog.Application.IntegrationEvents;
using Catalog.Application.IntegrationEvents.Products;
using Catalog.Domain.Entities.InventoryItems.Events;
using Catalog.Domain.Entities.Products.Events;
using EdaMicroEcommerce.Application.Outbox;
using EdaMicroEcommerce.Domain.BuildingBlocks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Catalog.Infra;

public class DomainEventsInterceptor : SaveChangesInterceptor
{
    // <WARNING> Toda essa estrutura modular do OUTBOX ficou mal implementada de uma forma 
    // que a abstração e a generalização esta ruim, exigindo uma repetição de codigo
    private readonly Dictionary<Type, Func<IDomainEvent, OutboxIntegrationEvent<EventType>>> _factoryDictionary = new()
    {
        { typeof(ProductDeactivatedEvent), e => ProductIntegrationFactory.FromDomain((ProductDeactivatedEvent)e) },
        { typeof(ProductReservedEvent), e => ProductIntegrationFactory.FromDomain((ProductReservedEvent) e) }
    };

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        return SavingChangesAsync(eventData, result).GetAwaiter().GetResult();
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result,
        // TODO: Lidar melhor com cancellation tokens
        CancellationToken cancellationToken = new CancellationToken())
    {
        var currentActivity = Activity.Current;
        
        var context = eventData.Context;

        if (context is null)
            throw new ArgumentException("Expected to receive a context here.");

        var outbox = context.Set<OutboxIntegrationEvent<EventType>>();

        var entries = context!.ChangeTracker.Entries()
            .Where(evt => evt.State is EntityState.Added or EntityState.Modified or EntityState.Unchanged)
            .Select(e => e.Entity);

        var entriesAggregate = entries.OfType<IAggregateRoot>().ToList();
        var domainEvents = entriesAggregate.SelectMany(e => e.GetDomainEvents());

        foreach (var domainEvt in domainEvents)
        {
            if (_factoryDictionary.TryGetValue(domainEvt.GetType(), out var factoryFunc))
            {
                var outboxObject = factoryFunc(domainEvt);

                if (currentActivity is not null) {
                    outboxObject.TraceId = currentActivity.TraceId.ToHexString();
                    outboxObject.SpanId = currentActivity.SpanId.ToHexString();
                }
                
                outbox.Add(outboxObject);
            }
        }

        foreach (var entry in entriesAggregate)
            entry.ClearDomainEvents();

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}