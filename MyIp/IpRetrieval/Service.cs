using Microsoft.Extensions.Options;

namespace MyIp.IpRetrieval;

public interface IIpRetrievalService
{
    Task<string?> CurrentIpAddress();
}

public class IpifyIpRetrieval : IIpRetrievalService
{
    private readonly HttpClient _httpClient;
    private readonly IOptionsMonitor<Ipify> _options;

    public IpifyIpRetrieval(HttpClient httpClient, IOptionsMonitor<Ipify> options)
    {
        _httpClient = httpClient;
        _options = options;
    }

    public async Task<string?> CurrentIpAddress()
    {
        var response = await _httpClient.GetAsync(_options.CurrentValue.QueryUri);

        return response.IsSuccessStatusCode
            ? await response.Content.ReadAsStringAsync()
            : null;
    }
}

public class Ipify
{
    public required Uri QueryUri { get; init; }
}