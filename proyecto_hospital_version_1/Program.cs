using Microsoft.EntityFrameworkCore;
using proyecto_hospital_version_1.Components;
using proyecto_hospital_version_1.Data;
using Hospital.Api.Data.Entities;
using proyecto_hospital_version_1.Services;
using proyecto_hospital_version_1.Helpers;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ========== DB CONTEXTS ==========
// Solo el DbContext principal (Identity)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// ========== MUD BLAZOR ==========
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.BottomRight;
});

// ========== HTTP CLIENT PARA API (CORREGIDO) ==========
builder.Services.AddHttpClient<DashboardService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7032/");
    client.Timeout = TimeSpan.FromSeconds(30); // Timeout de 30 segundos
});

builder.Services.AddHttpClient<IPacienteApiService, PacienteApiService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7032/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<ISolicitudQuirurgicaApiService, SolicitudQuirurgicaApiService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7032/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<ISolicitudProcedimientoApiService, SolicitudProcedimientoApiService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7032/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<IConsentimientoInformadoService, ConsentimientoInformadoService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7032/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<PriorizacionApiService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7032/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<IProcedimientoService, ProcedimientoService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7032/");
    client.Timeout = TimeSpan.FromSeconds(30);
});


builder.Services.AddHttpClient<IEspecialidadHospital, EspecialidadHospitalService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7032/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<IDiagnosticoService, DiagnosticoService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7032/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// ========== SWEET ALERTS ==========
builder.Services.AddScoped<SweetAlertHelper>();

// ========== BLAZOR ==========
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();