using System.Net;
using Microsoft.Extensions.Options;
using MyIp.AzureDns;
using MyIp.IpRetrieval;

namespace MyIp.SyncService;

public class IpSyncBackgroundService : BackgroundService
{
    private readonly ILogger<IpSyncBackgroundService> _logger;
    private readonly IServiceProvider _serviceCollection;
    private readonly IState _state;
    private readonly IOptionsMonitor<SyncSettings> _syncSettings;

    public IpSyncBackgroundService(ILogger<IpSyncBackgroundService> logger, IServiceProvider serviceCollection, IState state, IOptionsMonitor<SyncSettings> syncSettings)
    {
        _logger = logger;
        _serviceCollection = serviceCollection;
        _state = state;
        _syncSettings = syncSettings;
    }
    
    
    private ICurrentIpAddress IpifyService
    {
        get
        {
            return _serviceCollection.GetRequiredService<ICurrentIpAddress>();
        }    
    }

    private AzureDnsService AzureDnsService
    {
        get
        {
            return _serviceCollection.GetRequiredService<AzureDnsService>();
        }
    }


    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var currentIp = await IpifyService.CurrentIpAddressAsync(cancellationToken);
        var ipFromDns = await AzureDnsService.CurrentARecordValues(cancellationToken);
        
        _logger.LogInformation("Startup completed. CurrentIp: '{CurrentIp}'. AzureDnsIp: '{AzureDnsIp}'", currentIp, ipFromDns);
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
            _logger.LogInformation("DoSync disabled");
        }
    }

    private async Task DoSync(CancellationToken cancellationToken)
    {
        await IpifyService.CurrentIpAddressAsync(cancellationToken);
        await AzureDnsService.CurrentARecordValues(cancellationToken);

        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }
        
        if (CurrentIpAddressIsKnownWithoutError() && IpAddressChanged())
        {
            _logger.LogInformation("Ip address changed updating DNS to {NewIp}", _state.CurrentIpAddress);
            await AzureDnsService.Update(_state.CurrentIpAddress!);
        }
    }

    private bool CurrentIpAddressIsKnownWithoutError()
    {
        return _state is { ErrorCountIpRetrieval: 0, CurrentIpAddress: not null };
    }

    private bool IpAddressChanged()
    {
        return !_state?.CurrentIpAddress?.Equals(_state.InDnsZone) ?? false;
    }
}