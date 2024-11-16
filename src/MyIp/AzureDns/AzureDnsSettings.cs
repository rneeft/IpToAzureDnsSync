namespace MyIp.AzureDns;

public class AzureDnsSettings
{
    public required string SubscriptionId { get; init; }
    public required string ResourceGroupName { get; init; }
    public required string DnsZoneName { get; init; }
    public required string RecordSetName { get; init; }
}