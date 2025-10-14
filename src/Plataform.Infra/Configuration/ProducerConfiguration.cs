namespace EdaMicroEcommerce.Infra.Configuration;

public class ProducerConfiguration
{
    public string Topic { get; set; }
    public int Partitions { get; set; } = 1;
    public short ReplicaFactor { get; set; } = 1;
    public Dictionary<string, string> Config { get; set; }
}