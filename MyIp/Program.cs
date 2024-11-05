using MyIp;
using MyIp.AzureDns;
using MyIp.IpRetrieval;
using MyIp.SyncService;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddHttpClient()
    .AddSingleton<ICurrentIpAddress, IpifyService>()
    .Configure<IpifySettings>(builder.Configuration.GetSection("Ipify"));

builder.Services
    .AddTransient<AzureDnsService>()
    .Configure<AzureDnsSettings>(builder.Configuration.GetSection("AzureDns"));

builder.Services
    .Configure<SyncSettings>(builder.Configuration.GetSection("Sync"))
    .AddHostedService<IpSyncBackgroundService>();

builder.Services
    .AddSingleton<IState, InMemoryState>();

builder.Services.AddRazorPages();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();