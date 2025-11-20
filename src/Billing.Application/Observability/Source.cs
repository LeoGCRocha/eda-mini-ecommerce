using System.Diagnostics;

namespace Billing.Application.Observability;

public static class Source
{
    public static ActivitySource BillingSource = new ActivitySource("BillingSource");
}