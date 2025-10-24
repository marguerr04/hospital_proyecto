using Microsoft.EntityFrameworkCore;
using proyecto_hospital_version_1.Models;
using proyecto_hospital_version_1.Data.Hospital; // ¡IMPORTANTE: Usar HospitalDbContext!



namespace proyecto_hospital_version_1.Services
{
    public class DiagnosticoService : IDiagnosticoService
    {
        private readonly HospitalDbContext _context;

        public DiagnosticoService(HospitalDbContext context)
        {
            _context = context;
        }

        public async Task<List<Diagnostico>> GetDiagnosticosAsync()
        {
            return await _context.Diagnosticos
                .Select(d => new Diagnostico
                {
                    Id = d.Id,
                    CodigoCie = d.CodigoCie,
                    Nombre = d.Nombre
                })
                .Take(20)
                .ToListAsync();
        }

        public async Task<List<Diagnostico>> BuscarDiagnosticosAsync(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return await GetDiagnosticosAsync();

            return await _context.Diagnosticos
                .Where(d => d.Nombre.Contains(texto))
                .Select(d => new Diagnostico
                {
                    Id = d.Id,
                    CodigoCie = d.CodigoCie,
                    Nombre = d.Nombre
                })
                .Take(10)
                .ToListAsync();
        }
    }
}