using System.Net;

namespace MyIp;

public interface IState
{
    public List<IPAddress> UsedIpAddresses { get; }

    public DateTime LastRetrieval { get; set; }

    public IPAddress? CurrentIpAddress { get; }

    public IPAddress? InDnsZone { get; set; }
    
    public int ErrorCountIpRetrieval { get; set; }

    void NewIpAddress(IPAddress ipAddress);
}

public class InMemoryState : IState
{
    private readonly ILogger<InMemoryState> _logger;

    public InMemoryState(ILogger<InMemoryState> logger)
    {
        _logger = logger;
    }
    
    public List<IPAddress> UsedIpAddresses { get; } = [];

    public DateTime LastRetrieval { get; set; }

    public IPAddress? CurrentIpAddress { get; private set; }

    public IPAddress? InDnsZone { get; set; }
    
    public int ErrorCountIpRetrieval { get; set; }
    
    public void NewIpAddress(IPAddress ipAddress)
    {
        ErrorCountIpRetrieval = 0;

        if (IpHasChanged(ipAddress))
        {
            CurrentIpAddress = ipAddress;
            UsedIpAddresses.Add(ipAddress);
        }
    }

    private bool IpHasChanged(IPAddress ipAddress)
    {
        return !ipAddress.Equals(CurrentIpAddress);
    }
}
