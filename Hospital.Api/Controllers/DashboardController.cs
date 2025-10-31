using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyecto_hospital_version_1.Data;

namespace Hospital.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly HospitalDbContext _context;

        public DashboardController(HospitalDbContext context)
        {
            _context = context;
        }

        [HttpGet("percentil75")]
        public async Task<ActionResult<double>> GetPercentil75()
        {
            var lista = await _context.SOLICITUDES
                .GroupBy(s => s.PacienteId)
                .Select(g => g.Count())
                .ToListAsync();

            if (!lista.Any())
                return Ok(0);

            lista.Sort();
            int index = (int)Math.Ceiling(0.75 * lista.Count) - 1;
            index = Math.Max(0, index); // asegurar índice
            double percentil75 = lista[index];

            return Ok(percentil75);
        }

        [HttpGet("reduccion")]
        public Task<ActionResult<int>> GetReduccion()
        {
            return Task.FromResult<ActionResult<int>>(Ok(25));
        }

        [HttpGet("pendientes")]
        public async Task<ActionResult<int>> GetPendientes()
        {
            // Ejemplo: solicitudes pendientes en tabla SOLICITUDES
            var count = await _context.SOLICITUDES.CountAsync();
            return Ok(count);
        }

        [HttpGet("contactabilidad")]
        public async Task<ActionResult<Dictionary<string, double>>> GetContactabilidad()
        {
            var totalPacientes = await _context.PACIENTE.CountAsync(); // ✅ esto declara la variable

            var contactado = await _context.CONSENTIMIENTO_INFORMADO.CountAsync(c => c.Estado);
            var enProceso = await _context.CONSENTIMIENTO_INFORMADO.CountAsync(c => !c.Estado);

            var contactabilidad = new Dictionary<string, double>
            {
                { "Contactado", totalPacientes == 0 ? 0 : (double)contactado / totalPacientes * 100 },
                { "En proceso", totalPacientes == 0 ? 0 : (double)enProceso / totalPacientes * 100 },
                { "No contactado", totalPacientes == 0 ? 0 : (double)Math.Max(totalPacientes - contactado - enProceso, 0) / totalPacientes * 100 }
            };

            return Ok(contactabilidad); // ✅ asegura que siempre devuelves un valor
        }

        [HttpGet("procedimientos")]
        public async Task<ActionResult<Dictionary<string, int>>> GetProcedimientosPorTipo()
        {
            // Ejemplo: contar procedimientos por tipo
            var data = await _context.Procedimientos
                .GroupBy(p => p.Nombre)
                .Select(g => new { g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Key, x => x.Count);

            return Ok(data);
        }
    }
}
