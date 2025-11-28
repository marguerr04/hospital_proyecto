*using Hospital.Api.Data;
using Hospital.Api.Data.Services;
using Hospital.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BCrypt.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<HospitalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HospitalV4")));

builder.Services.AddControllers();

// ? Configuraci�n mejorada de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Hospital API", 
        Version = "v1",
        Description = "API para gesti�n hospitalaria"
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

// JWT configuration
var key = builder.Configuration["Jwt:Key"];
if (!string.IsNullOrEmpty(key))
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = builder.Configuration["Jwt:Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });
    builder.Services.AddAuthorization();
}

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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Forzar carga de controladores
var controllerCount = app.Services.GetService<Microsoft.AspNetCore.Mvc.Infrastructure.IActionDescriptorCollectionProvider>()
    ?.ActionDescriptors.Items.Count() ?? 0;
Console.WriteLine($"? {controllerCount} endpoints registrados");

// generar hash (ejecutar en un pequeño programa o REPL)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var hashMedico = BCrypt.Net.BCrypt.HashPassword("medico");
        var hashAdmin = BCrypt.Net.BCrypt.HashPassword("admin");
        Console.WriteLine(hashMedico);
        Console.WriteLine(hashAdmin);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al generar hashes: {ex.Message}");
    }
}

// ejemplo SQL orientativo (ajusta nombres de columnas y tabla)
// INSERT INTO USUARIO (Username, PasswordHash, Role, IsActive)
// VALUES ('medico1', '<HASH_MEDICO_GENERADO>', 'medico', 1);

// INSERT INTO USUARIO (Username, PasswordHash, Role, IsActive)
// VALUES ('admin', '<HASH_ADMIN_GENERADO>', 'administrador', 1);

app.Run();
