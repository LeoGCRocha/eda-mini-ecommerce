namespace EdaMicroEcommerce.Infra.Configuration;

public class MessageBrokerConfiguration
{
    public required string BootstrapServers  { get; set; }
    public Dictionary<string, ConsumerConfiguration> Consumers { get; set; }
    public Dictionary<string, ProducerConfiguration> Producers { get; set; }
}

public class ConsumerConfiguration
{
    public string GroupId { get; set; }
    public string Topic { get; set; }
    public Dictionary<string, string> Config { get; set; }
}

public class ProducerConfiguration
{
    public string Topic { get; set; }
    public int Partitions { get; set; } = 1;
    public short ReplicaFactor { get; set; } = 1;
    public Dictionary<string, string> Config { get; set; }
}