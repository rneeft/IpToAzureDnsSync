using Microsoft.Extensions.Options;
using System.Net;

namespace MyIp.IpRetrieval;

public interface ICurrentIPAddress
{
    Task<IPAddress?> CurrentIpAddress();
}

public class IpifyService : ICurrentIPAddress
{
    private readonly HttpClient _httpClient;
    private readonly IOptionsMonitor<IpifySettings> _options;
    private readonly InMemoryDatabase _inMemoryDatabase;

    public IpifyService(HttpClient httpClient, IOptionsMonitor<IpifySettings> options, InMemoryDatabase inMemoryDatabase)
    {
        _httpClient = httpClient;
        _options = options;
        _inMemoryDatabase = inMemoryDatabase;
    }

    public async Task<IPAddress?> CurrentIpAddress()
    {
        var response = await _httpClient.GetAsync(_options.CurrentValue.QueryUri);

        _inMemoryDatabase.LastRetrieveal = DateTime.UtcNow;

        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();

            if (IPAddress.TryParse(body, out IPAddress? iPAddress))
            {
                _inMemoryDatabase.CurrentIPAddress = iPAddress;
                return iPAddress;
            }
        }

        return null;
    }
}
