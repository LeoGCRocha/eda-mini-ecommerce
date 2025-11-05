using System.Diagnostics;

namespace Catalog.Application.Observability;

public static class Source
{
    public static readonly ActivitySource CatalogSource = new ActivitySource("CatalogSource");
}