using System.Net;

namespace MyIp;

public class InMemoryDatabase
{
    public List<IPAddress> UsedIpAddresses { get; } = [];

    public DateTime LastRetrieveal { get; set; }

    public IPAddress? CurrentIPAddress { get; set; }

    public IPAddress? InDNSZone { get; set; }
}
