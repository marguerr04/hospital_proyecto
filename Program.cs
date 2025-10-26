using proyecto_hospital_version_1.Data;
using proyecto_hospital_version_1.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Servicios de base de datos
builder.Services.AddDbContext<HospitalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HospitalV4")));

// Servicios de MudBlazor
builder.Services.AddMudServices();
builder.Services.AddScoped<DashboardService>();

// Razor Pages y Blazor Server
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();

// Pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting(); // <- MUY IMPORTANTE

app.MapBlazorHub();
app.MapFallbackToPage("/_Host"); // <- Punto de entrada Blazor Server

// Inicialización de la BD
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HospitalDbContext>();
    db.Database.EnsureCreated();

    if (!db.PACIENTE.Any())
    {
        var p = new Paciente
        {
            Rut = "11.111.111-1",
            PrimerNombre = "Paciente Demo",
            ApellidoPaterno = "Demo",
            ApellidoMaterno = "Ejemplo",
            FechaNacimiento = DateTime.Now.AddYears(-30),
            Sexo = "M",
            TelefonoMovil = "999999999",
            Mail = "demo@hospital.cl",
            TelefonoFijo = "222222222"
        };

        db.PACIENTE.Add(p);
        db.SaveChanges();
    }
}

app.Run();
