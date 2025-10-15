using Orders.Application.IntegrationEvents;
using Orders.Domain.Entities.Events;

namespace Orders.Infra;

using EdaMicroEcommerce.Application.Outbox;
using EdaMicroEcommerce.Domain.BuildingBlocks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;


public class DomainEventsInterceptor : SaveChangesInterceptor
{
    // <WARNING> Toda essa estrutura modular do OUTBOX ficou mal implementada de uma forma 
    // que a abstração e a generalização esta ruim, exigindo uma repetição de codigo
    // Acoplar IRequest na Entidade também fez o código ficar mal organizado, deveria ter pensando a mensagem e a entidade de outbox
    // como objetos distintos
    private readonly Dictionary<Type, Func<IDomainEvent, OutboxIntegrationEvent<EventType>>> _factoryDictionary = new()
    {
        { typeof(OrderCreatedEvent), e => OrderIntegrationFactory.FromDomain((OrderCreatedEvent) e)  }
    };

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        return SavingChangesAsync(eventData, result).GetAwaiter().GetResult();
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var context = eventData.Context;

        if (context is null)
            throw new ArgumentException("Expected to receive a context here.");

        var outbox = context.Set<OutboxIntegrationEvent<EventType>>();

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