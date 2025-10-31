using System.Text.Json;
using Orders.Application.IntegrationEvents.Orders;
using Orders.Domain.Entities.Events;

namespace Orders.Application.IntegrationEvents;

public static class OrderIntegrationFactory
{
    public static OrderCreatedIntegration FromDomain(OrderCreatedEvent evt)
    {
        var payload = JsonSerializer.Serialize(evt);
        return new OrderCreatedIntegration(EventType.OrderCreated, payload);
    }
}