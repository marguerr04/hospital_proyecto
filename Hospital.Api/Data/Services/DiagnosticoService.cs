using Hospital.Api.Data;
using Hospital.Api.Data.Entities;
using Hospital.Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Api.Services
{
    public class DiagnosticoService : IDiagnosticoService
    {
        private readonly HospitalDbContext _context;

        public DiagnosticoService(HospitalDbContext context)
        {
            _context = context;
        }

        public async Task<List<DiagnosticoDto>> GetDiagnosticosAsync()
        {
            return await _context.DIAGNOSTICO
                .Include(d => d.MapeosGes)
                .Select(d => new DiagnosticoDto
                {
                    Id = d.Id,
                    Nombre = d.Nombre,
                    CodigoCie = d.CodigoCie,
                    EsGes = d.MapeosGes.Any()
                })
                .ToListAsync();
        }

        public async Task<List<DiagnosticoDto>> GetDiagnosticosGesAsync()
        {
            return await _context.DIAGNOSTICO
                .Include(d => d.MapeosGes)
                .Where(d => d.MapeosGes.Any())
                .Select(d => new DiagnosticoDto
                {
                    Id = d.Id,
                    Nombre = d.Nombre,
                    CodigoCie = d.CodigoCie,
                    EsGes = true
                })
                .ToListAsync();
        }

        public async Task<List<DiagnosticoDto>> BuscarDiagnosticosAsync(string? texto = null)
        {
            var query = _context.DIAGNOSTICO
                .Include(d => d.MapeosGes)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(texto))
            {
                query = query.Where(d => d.Nombre.Contains(texto) || d.CodigoCie.Contains(texto));
            }

            return await query
                .Select(d => new DiagnosticoDto
                {
                    Id = d.Id,
                    Nombre = d.Nombre,
                    CodigoCie = d.CodigoCie,
                    EsGes = d.MapeosGes.Any()
                })
                .ToListAsync();
        }

        public async Task<DiagnosticoDto?> GetDiagnosticoByIdAsync(int id)
        {
            var diagnostico = await _context.DIAGNOSTICO
                .Include(d => d.MapeosGes)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (diagnostico == null) return null;

            return new DiagnosticoDto
            {
                Id = diagnostico.Id,
                Nombre = diagnostico.Nombre,
                CodigoCie = diagnostico.CodigoCie,
                EsGes = diagnostico.MapeosGes.Any()
            };
        }
    }
}