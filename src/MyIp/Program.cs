using System.Net;
using Azure.ResourceManager.Dns.Models;
using Azure.ResourceManager.Dns;
using Azure;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.Core;
using MyIp;

var client = new HttpClient();
var apiUrl = "https://api.ipify.org";
Settings settings;

try
{
    settings = new Settings();
}
catch (Exception ex)
{
    Console.WriteLine(ex);
    throw;
}

if (settings.RunContinuously)
{
    await RunContinuouslyAsync();
}
else
{
    await RunOnceAsync();
}

async Task RunContinuouslyAsync()
{
    var ipFromDNS = await CurrentIpAddressFromDns();
    var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));
    do
    {
        var currentIp = await GetIpAddressAsync();

        if (currentIp is not null && currentIp != ipFromDNS && await UpdateDnsAsync(currentIp))
        {
            ipFromDNS = currentIp;
        }
    }
    while(await timer.WaitForNextTickAsync());
}

async Task RunOnceAsync()
{
    var ipFromDNS = await CurrentIpAddressFromDns();
    var currentIp = await GetIpAddressAsync();

    if (currentIp is not null && currentIp != ipFromDNS)
    {
        await UpdateDnsAsync(currentIp);
    }
}

async Task<string?> GetIpAddressAsync()
{
    try
    {
        var response = await client.GetAsync(apiUrl);
        response.EnsureSuccessStatusCode();
        var currentIp = await response.Content.ReadAsStringAsync();

        if (settings.DebugLogging)
        {
            Console.WriteLine($"Current ip is {currentIp}");
        }

        return currentIp;
    }
    catch
    {
        Console.WriteLine("Unable to retrieve IP.");
        return null;
    }
}

async Task<string?> CurrentIpAddressFromDns()
{
    var dnsARecordCollection = GetAuthenticatedDnsARecordCollection();
    var dnsRecord = await dnsARecordCollection.GetAsync(settings.ARecordName);

    var ipFromDNS =  dnsRecord.Value.Data.DnsARecords.FirstOrDefault()?.IPv4Address?.ToString();

    if (settings.DebugLogging)
    {
        Console.WriteLine($"Ip from DNS is {ipFromDNS ?? "NULL"}");
    }

    return ipFromDNS;
}

DnsARecordCollection GetAuthenticatedDnsARecordCollection()
{
    var credential = new ClientSecretCredential(settings.TenantId, settings.ClientId, settings.ClientSecret);
    var armClient = new ArmClient(credential);
    var dnsZoneResource = armClient.GetDnsZoneResource(new ResourceIdentifier(settings.ResourceId));
    return dnsZoneResource.GetDnsARecords();
}

async Task<bool> UpdateDnsAsync(string ipAddress)
{
    if (settings.DebugLogging)
    {
        Console.WriteLine($"Updating DNS to ip {ipAddress}.");
    }
    try
    {

        var dnsARecordData = new DnsARecordData 
        { 
            TtlInSeconds = (long)TimeSpan.FromHours(1).TotalSeconds 
        };
        dnsARecordData.DnsARecords.Add(new DnsARecordInfo 
        {
            IPv4Address = IPAddress.Parse(ipAddress)
        });

        var dnsARecordCollection = GetAuthenticatedDnsARecordCollection();
        await dnsARecordCollection.CreateOrUpdateAsync(WaitUntil.Completed, settings.ARecordName, dnsARecordData);
        return true;
    }
    catch
    {
        Console.WriteLine("Unable to update DNS.");
        return false;
    }

}
