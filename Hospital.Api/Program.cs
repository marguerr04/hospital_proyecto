using Hospital.Api.Data;
using Hospital.Api.Data.Services;
using Hospital.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<HospitalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HospitalV4")));

builder.Services.AddControllers();

// ? Configuración mejorada de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Hospital API", 
        Version = "v1",
        Description = "API para gestión hospitalaria"
    });
    
    // Ignorar errores de tipos duplicados
    c.CustomSchemaIds(type => type.FullName);
});

// Servicios
builder.Services.AddScoped<SolicitudQuirurgicaRealService>();
builder.Services.AddScoped<ISolicitudQuirurgicaService, SolicitudQuirurgicaRealService>();
builder.Services.AddScoped<IDiagnosticoService, DiagnosticoService>();
builder.Services.AddScoped<IAuditoriaPriorizacionService, AuditoriaPriorizacionService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient",
        policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

// Swagger solo en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hospital API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazorClient");
app.UseAuthorization();
app.MapControllers();

// Forzar carga de controladores
var controllerCount = app.Services.GetService<Microsoft.AspNetCore.Mvc.Infrastructure.IActionDescriptorCollectionProvider>()
    ?.ActionDescriptors.Items.Count() ?? 0;
Console.WriteLine($"? {controllerCount} endpoints registrados");

app.Run();
