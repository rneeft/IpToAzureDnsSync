using Microsoft.Extensions.Options;
using System.Net;

namespace MyIp.IpRetrieval;

public class IpifyService : ICurrentIpAddress
{
    private readonly HttpClient _httpClient;
    private readonly IOptionsMonitor<IpifySettings> _options;
    private readonly IState _inMemoryState;

    public IpifyService(HttpClient httpClient, IOptionsMonitor<IpifySettings> options, IState inMemoryState)
    {
        _httpClient = httpClient;
        _options = options;
        _inMemoryState = inMemoryState;
    }

    public async Task<IPAddress?> CurrentIpAddressAsync(CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(_options.CurrentValue.QueryUri, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            if (IPAddress.TryParse(body, out var iPAddress))
            {
                _inMemoryState.NewIpAddress(iPAddress);
                return iPAddress;
            }
        }

        _inMemoryState.ErrorCountIpRetrieval++;
        return null;
    }
}
