using System.Net;

namespace MyIp.IpRetrieval;

public interface ICurrentIpAddress
{
    Task<IPAddress?> CurrentIpAddressAsync(CancellationToken cancellationToken);
}