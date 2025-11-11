using System.Text;
using KafkaFlow;

namespace EdaMicroEcommerce.Application;

public static class ContextPropagationHelper
{
    public static IEnumerable<string> ExtractHeader(IMessageHeaders headers, string key)
    {
        return from header in headers
            where header.Key.Equals(key, StringComparison.OrdinalIgnoreCase)
            select Encoding.UTF8.GetString(header.Value);
    }
}