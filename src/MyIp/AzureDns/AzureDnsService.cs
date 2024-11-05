using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Dns;
using Azure.ResourceManager.Dns.Models;
using Microsoft.Extensions.Options;
using System.Net;

namespace MyIp.AzureDns;

public class AzureDnsService
{
    private readonly IOptionsMonitor<AzureDnsSettings> _options;
    private readonly InMemoryDatabase _inMemoryDatabase;

    public AzureDnsService(IOptionsMonitor<AzureDnsSettings> options, InMemoryDatabase inMemoryDatabase)
    {
        _options = options;
        _inMemoryDatabase = inMemoryDatabase;
    }

    public async Task<IPAddress?> CurrentARecordValues()
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
            var ipInDns = dnsARecord.Data.DnsARecords
                .Select(x => x.IPv4Address)
                .FirstOrDefault();

            _inMemoryDatabase.InDnsZone = ipInDns;
            return ipInDns;
        }

        _inMemoryDatabase.InDnsZone = null;
        return null;
    }

    public async Task Update(IPAddress newIpAddress)
    {
        var credentials = new DefaultAzureCredential();
        var client = new ArmClient(credentials);

        var dnsZoneResourceId = DnsZoneResource.CreateResourceIdentifier(
            _options.CurrentValue.SubscriptionId,
            _options.CurrentValue.ResourceGroupName,
            _options.CurrentValue.DnsZoneName);

        var dnsZone = client.GetDnsZoneResource(dnsZoneResourceId);

        DnsARecordResource dnsARecord = await dnsZone.GetDnsARecords().GetAsync(_options.CurrentValue.RecordSetName);

        dnsARecord.Data.DnsARecords.Clear();
        dnsARecord.Data.DnsARecords.Add(new DnsARecordInfo { IPv4Address = newIpAddress });

        await dnsARecord.UpdateAsync(dnsARecord.Data);
    }
}
