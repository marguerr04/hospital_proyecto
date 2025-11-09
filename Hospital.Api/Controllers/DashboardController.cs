using Hospital.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        //  Percentil 75 basado SOLO en SOLICITUD_QUIRURGICA
        [HttpGet("percentil75")]
        public async Task<ActionResult<double>> GetPercentil75()
        {
            var lista = await _context.SOLICITUD_QUIRURGICA // ahora debe pasar po CI para saber idpaciente
                .Include(s => s.Consentimiento)
                .GroupBy(s => s.Consentimiento.PacienteId)
                .Select(g => g.Count())
                .ToListAsync();

            if (!lista.Any())
                return Ok(0);

            lista.Sort();
            int index = (int)Math.Ceiling(0.75 * lista.Count) - 1;
            index = Math.Max(0, index);
            double percentil75 = lista[index];

            return Ok(percentil75);
        }

        //  Valor fijo de ejemplo (meta del hospital)
        [HttpGet("reduccion")]
        public Task<ActionResult<int>> GetReduccion()
        {
            return Task.FromResult<ActionResult<int>>(Ok(25));
        }

        //  Total de solicitudes quirurgicas registradas
        [HttpGet("pendientes")]
        public async Task<ActionResult<int>> GetPendientes()
        {
            var count = await _context.SOLICITUD_QUIRURGICA.CountAsync();
            return Ok(count);
        }

        //  Contactabilidad basada en consentimiento informado
        [HttpGet("contactabilidad")]
        public async Task<ActionResult<Dictionary<string, double>>> GetContactabilidad()
        {
            var totalPacientes = await _context.PACIENTE.CountAsync();

            var contactado = await _context.CONSENTIMIENTO_INFORMADO.CountAsync(c => c.Estado);
            var enProceso = await _context.CONSENTIMIENTO_INFORMADO.CountAsync(c => !c.Estado);

            var contactabilidad = new Dictionary<string, double>
            {
                { "Contactado", totalPacientes == 0 ? 0 : (double)contactado / totalPacientes * 100 },
                { "En proceso", totalPacientes == 0 ? 0 : (double)enProceso / totalPacientes * 100 },
                { "No contactado", totalPacientes == 0 ? 0 : (double)Math.Max(totalPacientes - contactado - enProceso, 0) / totalPacientes * 100 }
            };

            return Ok(contactabilidad);
        }

        //  Conteo de procedimientos por nombre
        [HttpGet("procedimientos")]
        public async Task<ActionResult<Dictionary<string, int>>> GetProcedimientosPorTipo()
        {
            var data = await _context.PROCEDIMIENTO
                .GroupBy(p => p.Nombre)
                .Select(g => new { g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Key, x => x.Count);

            return Ok(data);
        }
    }
}
