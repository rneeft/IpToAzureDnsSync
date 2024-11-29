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
    private readonly ILogger<AzureDnsService> _logger;
    private readonly IState _inMemoryState;

    public AzureDnsService(IOptionsMonitor<AzureDnsSettings> options, ILogger<AzureDnsService> logger, IState inMemoryState)
    {
        _options = options;
        _logger = logger;
        _inMemoryState = inMemoryState;
    }

    public async Task<bool> CanAccessAsync(CancellationToken cancellationToken)
    {
        try
        {
            var dnsZone = CreateDnsZone();

            if (!string.IsNullOrWhiteSpace(_options.CurrentValue.RecordSetName))
            {
                await dnsZone.GetDnsARecords()
                    .GetAsync(_options.CurrentValue.RecordSetName, cancellationToken);               
            }

            foreach (var name in _options.CurrentValue.RecordSetNames)
            {
                await dnsZone.GetDnsARecords()
                    .GetAsync(_options.CurrentValue.RecordSetName, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cannot access Azure DNS Zone");   
            return false;
        }

        return true;
    }

    public async Task<IPAddress?> CurrentARecordValues(CancellationToken cancellationToken)
    {
        var dnsZone = CreateDnsZone();

        DnsARecordResource dnsARecord = await dnsZone.GetDnsARecords().GetAsync(_options.CurrentValue.RecordSetName,cancellationToken);

        if (dnsARecord.HasData)
        {
            var ipInDns = dnsARecord.Data.DnsARecords
                .Select(x => x.IPv4Address)
                .FirstOrDefault();

            _inMemoryState.InDnsZone = ipInDns;
            return ipInDns;
        }

        _inMemoryState.InDnsZone = null;
        return null;
    }

    public async Task Update(IPAddress newIpAddress)
    {
        var dnsZone = CreateDnsZone();

        if (!string.IsNullOrWhiteSpace(_options.CurrentValue.RecordSetName))
        {
            await UpdateIp(dnsZone, _options.CurrentValue.RecordSetName, newIpAddress);
        }

        foreach (var name in _options.CurrentValue.RecordSetNames)
        {
            await UpdateIp(dnsZone, name, newIpAddress);
        }
    }

    private DnsZoneResource CreateDnsZone()
    {
        var credentials = new DefaultAzureCredential();
        var client = new ArmClient(credentials);

        var dnsZoneResourceId = DnsZoneResource.CreateResourceIdentifier(
            _options.CurrentValue.SubscriptionId,
            _options.CurrentValue.ResourceGroupName,
            _options.CurrentValue.DnsZoneName);
        
        var dnsZone = client.GetDnsZoneResource(dnsZoneResourceId);

        return dnsZone;
    }

    private async Task UpdateIp(DnsZoneResource dnsZone, string aRecord, IPAddress ipAddress)
    {
        _logger.LogInformation("Updating '{ARecord}' to '{IPAddress}'", aRecord, ipAddress);
        DnsARecordResource dnsARecord = await dnsZone.GetDnsARecords().GetAsync(aRecord);

        dnsARecord.Data.DnsARecords.Clear();
        dnsARecord.Data.DnsARecords.Add(new DnsARecordInfo { IPv4Address = ipAddress });

        await dnsARecord.UpdateAsync(dnsARecord.Data);
    }
}
