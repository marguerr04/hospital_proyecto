using Hospital.Api.Data;
using Hospital.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EspecialidadController : ControllerBase
    {
        private readonly HospitalDbContext _context;

        public EspecialidadController(HospitalDbContext context)
        {
            _context = context;
        }

        // Obtener todas las especialidades (DEVUELVE DTOs)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EspecialidadDto>>> GetEspecialidades()
        {
            var especialidades = await _context.ESPECIALIDAD
                .OrderBy(e => e.Nombre)
                .Select(e => new EspecialidadDto
                {
                    Id = e.Id,
                    Nombre = e.Nombre
                })
                .ToListAsync();

            return Ok(especialidades);
        }

        // Buscar por texto (DEVUELVE DTOs)
        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<EspecialidadDto>>> BuscarEspecialidades([FromQuery] string? texto)
        {
            var query = _context.ESPECIALIDAD.AsQueryable();

            if (!string.IsNullOrWhiteSpace(texto))
            {
                query = query.Where(e => e.Nombre.Contains(texto));
            }

            var resultados = await query
                .OrderBy(e => e.Nombre)
                .Take(20)
                .Select(e => new EspecialidadDto
                {
                    Id = e.Id,
                    Nombre = e.Nombre
                })
                .ToListAsync();

            return Ok(resultados);
        }
    }
}