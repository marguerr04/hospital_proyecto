// Hospital.Api/Data/Services/ProfesionalService.cs
using Hospital.Api.Data;
using Hospital.Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Api.Data.Services
{
    public class ProfesionalService
    {
        private readonly HospitalDbContext _db;
        public ProfesionalService(HospitalDbContext db) => _db = db;

        public async Task<List<ProfesionalDto>> ListAsync()
        {
            return await _db.PROFESIONAL
                // .Include(p => p.RolHospital)        // ajusta a tus nav props reales
                // .Include(p => p.Especialidad)       // idem
                .Select(p => new ProfesionalDto
                {
                    Id = p.Id,
                    NombreCompleto = p.primerNombre + " " + p.primerApellido,
                    RutCompleto = p.rut + "-" + p.dv,
                    // Rol = p.RolHospital.Nombre,      // ajusta según la propiedad real
                    // Especialidad = p.Especialidad.Nombre // ajusta según la propiedad real
                })
                .ToListAsync();
        }
    }
}