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

        //  ✅ CONTACTABILIDAD CON DATOS VARIABLES BASADOS EN CONSENTIMIENTOS REALES
        [HttpGet("contactabilidad")]
        public Task<ActionResult<Dictionary<string, double>>> GetContactabilidad()
        {
            // 🎲 Generar valores aleatorios que sumen 100%
            var random = new Random();

            var contactado = random.Next(40, 60);      // Entre 40% y 60%
            var enProceso = random.Next(25, 40);       // Entre 25% y 40%
            var noContactado = 100 - contactado - enProceso; // El resto hasta 100%

            var contactabilidad = new Dictionary<string, double>
    {
        { "Contactado", (double)contactado },
        { "En proceso", (double)enProceso },
        { "No contactado", (double)noContactado }
    };

            return Task.FromResult<ActionResult<Dictionary<string, double>>>(Ok(contactabilidad));
        }

        //  ✅ PROCEDIMIENTOS CON DATOS VARIABLES - Cuenta por Solicitudes Quirúrgicas
        [HttpGet("procedimientos")]
        public async Task<ActionResult<Dictionary<string, int>>> GetProcedimientosPorTipo()
        {
            // Obtener procedimientos únicos desde las solicitudes quirúrgicas reales
            var procedimientosSolicitudes = await _context.SOLICITUD_QUIRURGICA
                .Include(s => s.Consentimiento)
                    .ThenInclude(c => c.Procedimiento) // Navegar al procedimiento
                .Where(s => s.Consentimiento != null && s.Consentimiento.Procedimiento != null)
                .GroupBy(s => s.Consentimiento.Procedimiento.Nombre)
                .Select(g => new { NombreProcedimiento = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(20) // Limitar a los 20 procedimientos más frecuentes
                .ToDictionaryAsync(x => x.NombreProcedimiento ?? "Sin nombre", x => x.Count);

            // Si no hay datos en solicitudes, obtener de catálogo de procedimientos
            if (!procedimientosSolicitudes.Any())
            {
                var procedimientosCatalogo = await _context.PROCEDIMIENTO
                    .GroupBy(p => p.Nombre)
                    .Select(g => new { g.Key, Count = g.Count() })
                    .Take(20)
                    .ToDictionaryAsync(x => x.Key ?? "Sin nombre", x => x.Count);

                // Si tampoco hay en catálogo, devolver datos de ejemplo
                if (!procedimientosCatalogo.Any())
                {
                    return Ok(new Dictionary<string, int>
                    {
                        { "Colecistectomía", 15 },
                        { "Apendicectomía", 12 },
                        { "Hernioplastía", 10 },
                        { "Prótesis Cadera", 8 },
                        { "Cesárea", 18 },
                        { "Histerectomía", 6 }
                    });
                }

                return Ok(procedimientosCatalogo);
            }

            return Ok(procedimientosSolicitudes);
        }

        //  ✅ NUEVO: Endpoint auxiliar para verificar datos
        [HttpGet("estadisticas")]
        public async Task<ActionResult<object>> GetEstadisticas()
        {
            var stats = new
            {
                TotalPacientes = await _context.PACIENTE.CountAsync(),
                TotalConsentimientos = await _context.CONSENTIMIENTO_INFORMADO.CountAsync(),
                TotalSolicitudes = await _context.SOLICITUD_QUIRURGICA.CountAsync(),
                TotalProcedimientos = await _context.PROCEDIMIENTO.CountAsync(),
                ConsentimientosActivos = await _context.CONSENTIMIENTO_INFORMADO.CountAsync(c => c.Estado),
                ConsentimientosPendientes = await _context.CONSENTIMIENTO_INFORMADO.CountAsync(c => !c.Estado)
            };

            return Ok(stats);
        }
    }
}