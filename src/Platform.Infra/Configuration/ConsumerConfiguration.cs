namespace EdaMicroEcommerce.Infra.Configuration;

public class ConsumerConfiguration
{
    public string GroupId { get; set; }
    public string Topic { get; set; }
    public Dictionary<string, string> Config { get; set; }
}