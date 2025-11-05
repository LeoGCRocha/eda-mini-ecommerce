using System.Diagnostics;

namespace EdaMicroEcommerce.Api.Observability;

public static class Source
{
    public static readonly ActivitySource ApiSource = new ActivitySource("ApiSource");
}