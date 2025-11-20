using Microsoft.EntityFrameworkCore;
using proyecto_hospital_version_1.Components;
using proyecto_hospital_version_1.Data;
using proyecto_hospital_version_1.Models;
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

// ❌ ELIMINADO: HospitalDbContextLegacy

// ========== MUD BLAZOR ==========
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.BottomRight;
});

// ========== HTTP CLIENT PARA API ==========
builder.Services.AddHttpClient("ApiReal", client =>
{
    client.BaseAddress = new Uri("https://localhost:7032/");
});

builder.Services.AddScoped(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    return factory.CreateClient("ApiReal");
});

// ========== SERVICIOS MIGRADOS A API ==========
// Estos servicios ahora usan HttpClient para llamar a la API
builder.Services.AddScoped<IEspecialidadHospital, EspecialidadHospitalService>();
builder.Services.AddScoped<IDiagnosticoService, DiagnosticoService>();
builder.Services.AddScoped<IProcedimientoService, ProcedimientoService>(); // ✅ Migrado a API

// ========== SERVICIOS API REALES ==========
builder.Services.AddScoped<IPacienteApiService, PacienteApiService>();
builder.Services.AddScoped<ISolicitudQuirurgicaApiService, SolicitudQuirurgicaApiService>();
builder.Services.AddScoped<ISolicitudProcedimientoApiService, SolicitudProcedimientoApiService>();
builder.Services.AddScoped<IConsentimientoInformadoService, ConsentimientoInformadoService>();
builder.Services.AddScoped<PriorizacionApiService>();
builder.Services.AddScoped<DashboardService>();


// ========== ELIMINAR SERVICIOS LEGACY ==========
// ❌ COMENTAR O ELIMINAR ESTOS:
// builder.Services.AddScoped<ISolicitudQuirurgicaService, SolicitudQuirurgicaService>();

// ========== SWEET ALERTS ==========
builder.Services.AddScoped<SweetAlertHelper>();

// ========== BLAZOR ==========
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<DashboardService>();

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