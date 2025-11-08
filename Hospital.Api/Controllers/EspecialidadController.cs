using Hospital.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hospital.Api.Data.Entities;
namespace Hospital.Api.Controllers


{using Hospital.Api.Data.Entities;
    [ApiController]
    [Route("api/[controller]")]
    public class EspecialidadController : ControllerBase
    {
        private readonly HospitalDbContext _context;

        public EspecialidadController(HospitalDbContext context)
        {
            _context = context;
        }

        //  1. Obtener todas las especialidades
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Especialidad>>> GetEspecialidades()
        {
            var especialidades = await _context.ESPECIALIDAD
                .OrderBy(e => e.Nombre)
                .ToListAsync();

            return Ok(especialidades);
        }

        //  2. Buscar por texto
        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<Especialidad>>> BuscarEspecialidades([FromQuery] string? texto)
        {
            var query = _context.ESPECIALIDAD.AsQueryable();

            if (!string.IsNullOrWhiteSpace(texto))
            {
                query = query.Where(e => e.Nombre.Contains(texto));
            }

            var resultados = await query
                .OrderBy(e => e.Nombre)
                .Take(20)
                .ToListAsync();

            return Ok(resultados);
        }
    }
}
