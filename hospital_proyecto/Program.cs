using System.Globalization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using proyecto_hospital_version_1.Data;
using proyecto_hospital_version_1;

var culture = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HttpClient apuntará al API. Ajusta la URL si tu API no corre en 5227.
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5227/") });

builder.Services.AddMudServices();
builder.Services.AddScoped<DashboardService>(); // servicio cliente que usa HttpClient

await builder.Build().RunAsync();
