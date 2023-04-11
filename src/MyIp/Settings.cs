using System.Diagnostics;

namespace MyIp;

public sealed class Settings
{
    public Settings()
    {
        ClientSecret = Environment.GetEnvironmentVariable("clientSecret") ??
            throw new InvalidOperationException("Variable clientSecret not set, Unable to run");

        ClientId = Environment.GetEnvironmentVariable("clientId") ??
            throw new InvalidOperationException("Variable clientSecret not set, Unable to run");

        TenantId = Environment.GetEnvironmentVariable("tenantId") ??
            throw new InvalidOperationException("Variable clientSecret not set, Unable to run");

        SubscriptionId = Environment.GetEnvironmentVariable("subscriptionId") ??
            throw new InvalidOperationException("Variable clientSecret not set, Unable to run");

        ResourceGroupName = Environment.GetEnvironmentVariable("resourceGroupName") ??
            throw new InvalidOperationException("Variable clientSecret not set, Unable to run");

        DnsZoneName = Environment.GetEnvironmentVariable("dnsZoneName") ??
            throw new InvalidOperationException("Variable clientSecret not set, Unable to run");

        ARecordName = Environment.GetEnvironmentVariable("aRecordName") ??
            throw new InvalidOperationException("Variable clientSecret not set, Unable to run");

        RunContinuously = bool.TryParse(Environment.GetEnvironmentVariable("runContinuously"), out var runContinuously) && runContinuously;
        DebugLogging = bool.TryParse(Environment.GetEnvironmentVariable("DebugLogging"), out var debugLogging) && debugLogging;

        ResourceId = $@"/subscriptions/{SubscriptionId}/resourceGroups/{ResourceGroupName}/providers/Microsoft.Network/dnszones/{DnsZoneName}";
    }

    public string ClientSecret { get; }
    public string ClientId { get; }
    public string TenantId { get; }
    public string SubscriptionId { get; }
    public string ResourceGroupName { get; }
    public string DnsZoneName { get; }
    public string ARecordName { get; }
    public bool RunContinuously { get; }
    public string ResourceId { get; }
    public bool DebugLogging { get; }
}
