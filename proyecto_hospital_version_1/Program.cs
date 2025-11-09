using Microsoft.EntityFrameworkCore;
using proyecto_hospital_version_1.Components;
using proyecto_hospital_version_1.Data;
using proyecto_hospital_version_1.Models;
using proyecto_hospital_version_1.Services;
using MudBlazor.Services;
using proyecto_hospital_version_1.Data._Legacy; // NUEVO: Agregado para MudBlazor (Dashboard)
using MudBlazor.Services;


var builder = WebApplication.CreateBuilder(args);

// --- SECCIÓN DE SERVICIOS (builder.Services) ---

// TUS SERVICIOS (Mantenidos)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Tu DbContext para 'Default'
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Tu DbContext para 'HospitalV4' (ahora con las nuevas tablas), esto es para la integracion actual
// Comentario para integracion actual 
/*
builder.Services.AddDbContext<HospitalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HospitalV4")));
*/
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.BottomRight;
});

builder.Services.AddDbContext<HospitalDbContextLegacy>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HospitalV4")));


builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5227") });
builder.Services.AddScoped<DashboardService>();

// servicio de la api
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7032/") // mi puerto de la api es 7032 martin 
});

// Registrar el servicio
builder.Services.AddScoped<IDiagnosticoService, DiagnosticoService>();


// Tus servicios de lógica de negocio
builder.Services.AddScoped<IPacienteService, PacienteService>();
builder.Services.AddScoped<IEspecialidadHospital, EspecialidadHospitalService>();
builder.Services.AddScoped<IDiagnosticoService, DiagnosticoService>();
builder.Services.AddScoped<ISolicitudQuirurgicaService, SolicitudQuirurgicaService>();
// Diagnostico
builder.Services.AddScoped<IProcedimientoService, ProcedimientoService>();
// Service nuevo
builder.Services.AddScoped<IEspecialidadService, EspecialidadService>();









// NUEVOS SERVICIOS (de tu compañero para el Dashboard)
// builder.Services.AddMudServices(); // Para los componentes del Dashboard, de moento parece qeu dme dan problemas, lo dejo comentado
// builder.Services.AddScoped<DashboardService>(); , deberia verlo en el commit de dashboard si es posible integrarlo
builder.Services.AddRazorPages(); // Necesario para MapFallbackToPage
builder.Services.AddServerSideBlazor(); // Necesario para MapBlazorHub


// servicio api
builder.Services.AddScoped<ISolicitudQuirurgicaApiService, SolicitudQuirurgicaApiService>();




// --- FIN DE SERVICIOS ---




var app = builder.Build();

// --- SECCIÓN DE PIPELINE (app.Use... / app.Map...) ---

// TU CONFIGURACIÓN (Mantenida)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
// app.UseAntiforgery(); // INCORRECTO: Esta línea estaba aquí


// NUEVA CONFIGURACIÓN (de tu compañero)
app.UseRouting(); // Importante: Habilita el enrutamiento

// CORRECTO: Antiforgery DEBE ir DESPUÉS de UseRouting()
app.UseAntiforgery();


// TU MAPEO (Mantenido)
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// NUEVO MAPEO (de tu compañero para Blazor Server / Dashboard)
// app.MapBlazorHub(); // COMENTADO - Esta línea busca el _Host.cshtml
// app.MapFallbackToPage("/_Host"); // COMENTADO - Esta es la línea que da el error


// --- FIN DE PIPELINE ---


// TU CÓDIGO DE INICIALIZACIÓN (AHORA COMENTADO) 
/*
// Este bloque se ha comentado porque 'db.Database.EnsureCreated()' entra en 
// conflicto directo con el sistema de Migraciones ('Update-Database').
// Si usas Migraciones, SÓLO debes usar 'Update-Database' para crear y 
// actualizar la base de datos.
// Si descomentas esto, puedes tener errores o una base de datos corrupta.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated(); // <-- Esta es la línea problemática

    if (!db.Pacientes.Any())
    * {
        var p = new Paciente { Rut = "11.111.111-1", Nombre = "Paciente Demo" };
        db.Pacientes.Add(p);
        db.Solicitudes.Add(new Solicitud
        {
            Paciente = p,
            Diagnostico = "Dx demo",
            Procedencia = Procedencia.Ambulatorio,
            EsGes = false
        });
        db.SaveChanges();
    }
}
*/

// TU app.Run() (Debe ser la ÚLTIMA línea)
app.Run();

