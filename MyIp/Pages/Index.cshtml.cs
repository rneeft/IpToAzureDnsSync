using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyIp.AzureDns;
using MyIp.IpRetrieval;

namespace MyIp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly ICurrentIPAddress _ipRetrievalService;
    private readonly AzureDnsService _azureDnsService;

    public IndexModel(ILogger<IndexModel> logger, ICurrentIPAddress ipRetrievalService, AzureDnsService azureDnsService)
    {
        _logger = logger;
        _ipRetrievalService = ipRetrievalService;
        _azureDnsService = azureDnsService;
    }

    public async Task OnGet()
    {
        var ip = await _ipRetrievalService.CurrentIpAddress();
        var currentARecords = await _azureDnsService.CurrentARecordValues();
        ViewData["Ip"] = ip ?? "TILT";
        ViewData["FirstItem"] = currentARecords.FirstOrDefault()?.ToString() ?? "TILT";
    }
}