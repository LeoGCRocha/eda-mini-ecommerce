namespace EdaMicroEcommerce.Infra.Configuration;

public class MessageBrokerConfiguration
{
    public required string BootstrapServers  { get; set; }
    public Dictionary<string, ConsumerConfiguration> Consumers { get; set; }
    public Dictionary<string, ProducerConfiguration> Producers { get; set; }
}