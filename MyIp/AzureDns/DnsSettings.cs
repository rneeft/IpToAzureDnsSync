using System.Net;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Dns;
using Azure.ResourceManager.Dns.Models;
using Microsoft.Extensions.Options;

namespace MyIp.AzureDns;

public class AzureDnsService
{
    private readonly IOptionsMonitor<AzureDnsSettings> _options;

    public AzureDnsService(IOptionsMonitor<AzureDnsSettings> options)
    {
        _options = options;
    }

    public async Task<IPAddress[]> CurrentARecordValues()
    {
        var credentials = new DefaultAzureCredential();
        var client = new ArmClient(credentials);
        
        var dnsZoneResourceId = DnsZoneResource.CreateResourceIdentifier(
            _options.CurrentValue.SubscriptionId, 
            _options.CurrentValue.ResourceGroupName,
            _options.CurrentValue.DnsZoneName);
        
        var dnsZone = client.GetDnsZoneResource(dnsZoneResourceId);
        
        DnsARecordResource dnsARecord = await dnsZone.GetDnsARecords().GetAsync(_options.CurrentValue.RecordSetName);

        if (dnsARecord.HasData)
        {
            return dnsARecord.Data.DnsARecords
                .Select(x => x.IPv4Address)
                .ToArray();
        }

        return [];
    }
/*
    public async Task Update()
    {
        var credentials = new DefaultAzureCredential();
        var client = new ArmClient(credentials);
        
        var dnsZoneResourceId = DnsZoneResource.CreateResourceIdentifier(
            _options.CurrentValue.SubscriptionId, 
            _options.CurrentValue.ResourceGroupName,
            _options.CurrentValue.DnsZoneName);
        
        var dnsZone = client.GetDnsZoneResource(dnsZoneResourceId);

        // Get the DNS record set (A record in this case)
        

        // Update the record's IP address
        dnsARecord.Data.ARecords.Clear(); // Clear existing records
        dnsARecord.Data.ARecords.Add(new DnsARecordInfo { IPv4Address = newIpAddress }); // Add the new IP

        // Commit the changes
        await dnsARecord.UpdateAsync(dnsARecord.Data);
        
    }
    */
}

public class AzureDnsSettings
{
    public required string SubscriptionId { get; init; }
    public required string ResourceGroupName { get; init; }
    public required string DnsZoneName { get; init; }
    public required string RecordSetName { get; init; }
}