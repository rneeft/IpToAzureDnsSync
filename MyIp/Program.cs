using MyIp.AzureDns;
using MyIp.IpRetrieval;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddHttpClient()
    .AddSingleton<IIpRetrievalService, IpifyIpRetrieval>()
    .Configure<Ipify>(builder.Configuration.GetSection("Ipify"));

builder.Services
    .AddTransient<AzureDnsService>()
    .Configure<AzureDnsSettings>(builder.Configuration.GetSection("AzureDns"));

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();