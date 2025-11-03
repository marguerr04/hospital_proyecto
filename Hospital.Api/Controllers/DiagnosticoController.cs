using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyecto_hospital_version_1.Data;

namespace Hospital.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiagnosticoController : ControllerBase
    {
        private readonly HospitalDbContext _context;

        public DiagnosticoController(HospitalDbContext context)
        {
            _context = context;
        }

        // GET: api/diagnostico
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetDiagnosticos()
        {
            var diagnosticos = await _context.DIAGNOSTICO
                .Select(d => new
                {
                    d.Id,
                    d.Nombre
                })
                .Take(20)
                .ToListAsync();

            return Ok(diagnosticos);
        }

        // GET: api/diagnostico/buscar?texto=apendicitis
        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<object>>> BuscarDiagnosticos([FromQuery] string texto)
        {
            var query = _context.DIAGNOSTICO.AsQueryable();

            if (!string.IsNullOrWhiteSpace(texto))
                query = query.Where(d => d.Nombre.Contains(texto));

            var diagnosticos = await query
                .Select(d => new
                {
                    d.Id,
                    d.Nombre
                })
                .Take(10)
                .ToListAsync();

            return Ok(diagnosticos);
        }
    }
}
