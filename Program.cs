using proyecto_hospital_version_1.Components;
using Microsoft.EntityFrameworkCore;
using proyecto_hospital_version_1.Data;
<<<<<<< Updated upstream
=======
using MudBlazor.Services;
>>>>>>> Stashed changes

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

<<<<<<< Updated upstream
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
=======
// Base de datos paralela
builder.Services.AddDbContext<HospitalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HospitalV4")));

builder.Services.AddMudServices();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
>>>>>>> Stashed changes

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
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HospitalDbContext>();
    db.Database.EnsureCreated(); // asegura que la BD existe

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



