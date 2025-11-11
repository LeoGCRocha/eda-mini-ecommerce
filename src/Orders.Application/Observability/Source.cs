using System.Diagnostics;

namespace Orders.Application.Observability;

public static class Source
{
    public static readonly ActivitySource OrderSource = new ActivitySource("OrdersSource");
}