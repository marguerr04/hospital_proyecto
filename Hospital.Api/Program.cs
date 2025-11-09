using Hospital.Api.Data;
using Microsoft.EntityFrameworkCore;
using Hospital.Api.Data.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<HospitalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HospitalV4")));





builder.Services.AddControllers();

// 2 lineas agregada para testear el MVC y crear endpoints
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// para el serviceio
builder.Services.AddScoped<SolicitudQuirurgicaRealService>();



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient",
        policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});


var app = builder.Build();

// si swagger 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}





app.UseCors("AllowBlazorClient");
app.MapControllers();
app.Run();
