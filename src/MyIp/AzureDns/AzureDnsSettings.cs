namespace MyIp.AzureDns;

public class AzureDnsSettings
{
    public required string SubscriptionId { get; init; }
    public required string ResourceGroupName { get; init; }
    public required string DnsZoneName { get; init; }
    public string? RecordSetName { get; init; }
    public required string[] RecordSetNames { get; init; } 
}