using Microsoft.Extensions.Diagnostics.HealthChecks;
using MyIp;
using MyIp.AzureDns;

public class MyIpHealthCheck : IHealthCheck
{
    private readonly IState _state;
    private readonly AzureDnsService _azureDnsService;

    public MyIpHealthCheck(IState state, AzureDnsService azureDnsService)
    {
        _state = state;
        _azureDnsService = azureDnsService;
    }
    
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (_state.ErrorCountIpRetrieval > 1)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, "Unhealthy");
        }

        var canAccess = await _azureDnsService.CanAccessAsync(cancellationToken);

        return canAccess
            ? HealthCheckResult.Healthy("Healthy")
            : new HealthCheckResult(context.Registration.FailureStatus, "Unhealthy");
    }
}