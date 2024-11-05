using Microsoft.Extensions.Options;
using MyIp.AzureDns;
using MyIp.IpRetrieval;

namespace MyIp.SyncService;

public class IpSyncBackgroundService : BackgroundService
{
    private readonly ILogger<IpSyncBackgroundService> _options;
    private readonly IServiceProvider _serviceCollection;
    private readonly IState _state;

    private ICurrentIpAddress _ipifyService
    {
        get
        {
            return _serviceCollection.GetRequiredService<ICurrentIpAddress>();
        }    
    }

    private AzureDnsService _azureDnsService
    {
        get
        {
            return _serviceCollection.GetRequiredService<AzureDnsService>();
        }
    }
    private readonly IOptionsMonitor<SyncSettings> _syncSettings;

    public IpSyncBackgroundService(ILogger<IpSyncBackgroundService> options, IServiceProvider serviceCollection, IState state, IOptionsMonitor<SyncSettings> syncSettings)
    {
        _options = options;
        _serviceCollection = serviceCollection;
        _state = state;
        _syncSettings = syncSettings;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var currentIp = await _ipifyService.CurrentIpAddressAsync(cancellationToken);
        var ipFromDns = await _azureDnsService.CurrentARecordValues(cancellationToken);
        
        _options.LogInformation("Startup completed. CurrentIp: '{CurrentIp}'. AzureDnsIp: '{AzureDnsIp}'", currentIp, ipFromDns);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_syncSettings.CurrentValue.DoSync)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await DoSync(stoppingToken);
                await Task.Delay(_syncSettings.CurrentValue.Timeout, stoppingToken);
            }
        }
        else
        {
            _options.LogInformation("DoSync disabled");
        }
    }

    private async Task DoSync(CancellationToken cancellationToken)
    {
        await _ipifyService.CurrentIpAddressAsync(cancellationToken);
        await _azureDnsService.CurrentARecordValues(cancellationToken);

        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }
        
        if (_state is { ErrorCountIpRetrieval: 0, CurrentIpAddress: not null })
        {
            if (!_state.CurrentIpAddress.Equals(_state.InDnsZone))
            {
                await _azureDnsService.Update(_state.CurrentIpAddress);
            }
        }
    }
}