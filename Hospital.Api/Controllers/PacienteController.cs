using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hospital.Api.Data;
using Hospital.Api.Data.Entities;

namespace Hospital.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PacienteController : ControllerBase
    {
        private readonly HospitalDbContext _context;

        public PacienteController(HospitalDbContext context)
        {
            _context = context;
        }

        // GET: api/Paciente
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetPacientes([FromQuery] int? limit = 50)
        {
            // Validar que limit no sea null
            var limitValue = limit ?? 50;

            var pacientes = await _context.PACIENTE
                .Select(p => new
                {
                    p.Id,
                    p.Rut,
                    p.Dv,
                    RutCompleto = p.Rut + "-" + p.Dv,
                    p.PrimerNombre,
                    p.SegundoNombre,
                    p.ApellidoPaterno,
                    p.ApellidoMaterno,
                    NombreCompleto = (p.PrimerNombre + " " + (p.SegundoNombre ?? "") + " " +
                                     p.ApellidoPaterno + " " + (p.ApellidoMaterno ?? "")).Trim(),
                    p.FechaNacimiento,
                    Edad = DateTime.Today.Year - p.FechaNacimiento.Year,
                    p.Sexo,
                    p.TelefonoMovil,
                    p.TelefonoFijo,
                    p.Mail,
                    p.PRAIS
                })
                .Take(limitValue)  //  Usar variable no nullable
                .ToListAsync();

            return Ok(pacientes);
        }

        // GET: api/Paciente/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetPaciente(int id)
        {
            var paciente = await _context.PACIENTE
                .Where(p => p.Id == id)
                .Select(p => new
                {
                    p.Id,
                    p.Rut,
                    p.Dv,
                    RutCompleto = p.Rut + "-" + p.Dv,
                    p.PrimerNombre,
                    p.SegundoNombre,
                    p.ApellidoPaterno,
                    p.ApellidoMaterno,
                    NombreCompleto = (p.PrimerNombre + " " + (p.SegundoNombre ?? "") + " " +
                                     p.ApellidoPaterno + " " + (p.ApellidoMaterno ?? "")).Trim(),
                    p.FechaNacimiento,
                    Edad = DateTime.Today.Year - p.FechaNacimiento.Year,
                    p.Sexo,
                    p.TelefonoMovil,
                    p.TelefonoFijo,
                    p.Mail,
                    p.PRAIS,
                    // CORREGIDO: Usar los campos reales de UBICACION
                    Ubicaciones = p.Ubicaciones.Select(u => new
                    {
                        u.IdDomicilio,
                        u.FechaRegistro,
                        u.NomDireccion,      
                        u.NumDireccion,      
                        u.CiudadId,          
                        u.TipoViaId,         
                        //  Propiedad calculada si existe en el modelo
                        DireccionCompleta = u.NomDireccion + " " + u.NumDireccion
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (paciente == null)
            {
                return NotFound(new { message = $"Paciente con Id {id} no encontrado." });
            }

            return Ok(paciente);
        }

        // GET: api/Paciente/buscar?texto=juan&rut=12345678&dv=9
        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<object>>> BuscarPacientes(
            [FromQuery] string? texto,
            [FromQuery] string? rut,
            [FromQuery] string? dv)
        {
            var query = _context.PACIENTE.AsQueryable();

            // Filtrar por texto (nombre, apellido o RUT)
            if (!string.IsNullOrWhiteSpace(texto))
            {
                var busqueda = texto.ToLower();
                query = query.Where(p =>
                    p.PrimerNombre.ToLower().Contains(busqueda) ||
                    (p.SegundoNombre != null && p.SegundoNombre.ToLower().Contains(busqueda)) ||
                    p.ApellidoPaterno.ToLower().Contains(busqueda) ||
                    (p.ApellidoMaterno != null && p.ApellidoMaterno.ToLower().Contains(busqueda)) ||
                    p.Rut.Contains(busqueda)
                );
            }

            // Filtrar por RUT específico
            if (!string.IsNullOrWhiteSpace(rut))
            {
                query = query.Where(p => p.Rut == rut);
            }

            // Filtrar por DV específico
            if (!string.IsNullOrWhiteSpace(dv))
            {
                query = query.Where(p => p.Dv.ToUpper() == dv.ToUpper());
            }

            var resultados = await query
                .Select(p => new
                {
                    p.Id,
                    p.Rut,
                    p.Dv,
                    RutCompleto = p.Rut + "-" + p.Dv,
                    p.PrimerNombre,
                    p.SegundoNombre,
                    p.ApellidoPaterno,
                    p.ApellidoMaterno,
                    NombreCompleto = (p.PrimerNombre + " " + (p.SegundoNombre ?? "") + " " +
                                     p.ApellidoPaterno + " " + (p.ApellidoMaterno ?? "")).Trim(),
                    p.FechaNacimiento,
                    Edad = DateTime.Today.Year - p.FechaNacimiento.Year,
                    p.Sexo,
                    p.TelefonoMovil,
                    p.Mail
                })
                .Take(20)
                .ToListAsync();

            Console.WriteLine($"[DEBUG] Búsqueda de pacientes: texto='{texto}', rut='{rut}', dv='{dv}' - Resultados: {resultados.Count}");

            return Ok(resultados);
        }
    }
}