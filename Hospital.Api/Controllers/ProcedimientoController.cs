using Hospital.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hospital.Api.Data.Entities;
namespace Hospital.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProcedimientoController : ControllerBase
    {
        private readonly HospitalDbContext _context;

        public ProcedimientoController(HospitalDbContext context)
        {
            _context = context;
        }

        // 1. Obtener todos los procedimientos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetProcedimientos()
        {
            var procedimientos = await _context.PROCEDIMIENTO
                .Select(p => new
                {
                    p.Id,
                    p.Codigo,
                    p.Nombre,
                    p.Descripcion
                })
                .Take(50)
                .ToListAsync();

            return Ok(procedimientos);
        }

        // 2. Buscar por nombre o código
        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<object>>> BuscarProcedimientos(
            [FromQuery] string? texto)
        {
            var query = _context.PROCEDIMIENTO.AsQueryable();

            if (!string.IsNullOrWhiteSpace(texto))
            {
                query = query.Where(p =>
                    p.Nombre.Contains(texto) ||
                    p.Codigo.ToString().Contains(texto));
            }

            var resultados = await query
                .Select(p => new
                {
                    p.Id,
                    p.Codigo,
                    p.Nombre,
                    p.Descripcion
                })
                .Take(20)
                .ToListAsync();

            return Ok(resultados);
        }
    }
}
