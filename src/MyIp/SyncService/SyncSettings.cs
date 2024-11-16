namespace MyIp.SyncService;

public class SyncSettings
{
    public required bool DoSync { get; init; }
    public required TimeSpan Timeout { get; init; }
}