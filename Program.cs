using proyecto_hospital_version_1.Components;
using proyecto_hospital_version_1.Services;
using Microsoft.EntityFrameworkCore;
using proyecto_hospital_version_1.Data;
using proyecto_hospital_version_1.Data.Hospital;  // ?? este es el namespace donde pusiste HospitalDbContext
using proyecto_hospital_version_1.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
// Base de datos paralela
builder.Services.AddDbContext<HospitalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HospitalV4")));

builder.Services.AddScoped<IPacienteService, PacienteService>();

builder.Services.AddScoped<IEspecialidadHospital, EspecialidadHospitalService>();

builder.Services.AddScoped<IDiagnosticoService, DiagnosticoService>();

// generación de la solicitud
builder.Services.AddScoped<ISolicitudQuirurgicaService, SolicitudQuirurgicaService>();


var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();




using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated(); // asegura que la BD existe

    if (!db.Pacientes.Any())
    {
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




app.Run();



