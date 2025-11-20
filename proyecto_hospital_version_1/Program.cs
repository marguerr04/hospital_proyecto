using Microsoft.EntityFrameworkCore;
using proyecto_hospital_version_1.Components;
using proyecto_hospital_version_1.Data;
using proyecto_hospital_version_1.Models;
using proyecto_hospital_version_1.Services;
using proyecto_hospital_version_1.Helpers;
using MudBlazor.Services;
using proyecto_hospital_version_1.Data._Legacy;

var builder = WebApplication.CreateBuilder(args);

//
// ============================
//       REGISTRO SERVICIOS
// ============================
//

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


// ========== DB CONTEXTS ==========

// Tu DB default (Identity u otra interna)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Contexto Legacy (solo para transicionar)
builder.Services.AddDbContext<HospitalDbContextLegacy>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HospitalV4")));


// ========== MUD BLAZOR ==========
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.BottomRight;
});


// ========== HTTP CLIENT ÚNICO PARA API REAL ==========
builder.Services.AddHttpClient("ApiReal", client =>
{
    client.BaseAddress = new Uri("https://localhost:7032/");
});

// forma recomendada para inyectarlo:
builder.Services.AddScoped(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    return factory.CreateClient("ApiReal");
});


// ========== SERVICIOS LEGACY (seguirán hasta migración total) ==========
builder.Services.AddScoped<IEspecialidadHospital, EspecialidadHospitalService>();
builder.Services.AddScoped<IDiagnosticoService, DiagnosticoService>();
builder.Services.AddScoped<IProcedimientoService, ProcedimientoService>();
builder.Services.AddScoped<ISolicitudQuirurgicaService, SolicitudQuirurgicaService>();



// ========== SERVICIOS API REALES ==========
builder.Services.AddScoped<IPacienteApiService, PacienteApiService>();
builder.Services.AddScoped<ISolicitudQuirurgicaApiService, SolicitudQuirurgicaApiService>();
builder.Services.AddScoped<ISolicitudProcedimientoApiService, SolicitudProcedimientoApiService>();
builder.Services.AddScoped<IConsentimientoInformadoService, ConsentimientoInformadoService>();
builder.Services.AddScoped<PriorizacionApiService>();


// ========== SWEET ALERTS ==========
builder.Services.AddScoped<SweetAlertHelper>();


// ========== BLAZOR ==========
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<proyecto_hospital_version_1.Services.DashboardService>();

//
// ============================
//       BUILD & PIPELINE
// ============================
//

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
