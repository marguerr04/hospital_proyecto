// Hospital.Api/Controllers/ConsentimientoInformadoController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hospital.Api.Data;
using Hospital.Api.Data.Entities;
using Hospital.Api.Data.DTOs; // Agregar este using para el DTO

[Route("api/[controller]")]
[ApiController]
public class ConsentimientoInformadoController : ControllerBase
{
    private readonly HospitalDbContext _context;

    public ConsentimientoInformadoController(HospitalDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<int>> PostConsentimientoInformado([FromBody] ConsentimientoInformadoDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { message = "Datos de consentimiento no válidos.", errors = ModelState });
        }

        // Verificaciones de existencia
        var pacienteExistente = await _context.PACIENTE.FindAsync(dto.PacienteId);
        if (pacienteExistente == null)
        {
            return BadRequest(new { message = $"Paciente con Id {dto.PacienteId} no encontrado." });
        }

        var procedimientoExistente = await _context.PROCEDIMIENTO.FindAsync(dto.ProcedimientoId);
        if (procedimientoExistente == null)
        {
            return BadRequest(new { message = $"Procedimiento con Id {dto.ProcedimientoId} no encontrado." });
        }

        var lateralidadExistente = await _context.LATERALIDAD.FindAsync(dto.LateralidadId);
        if (lateralidadExistente == null)
        {
            return BadRequest(new { message = $"Lateralidad con Id {dto.LateralidadId} no encontrada." });
        }

        var extremidadExistente = await _context.EXTREMIDAD.FindAsync(dto.ExtremidadId);
        if (extremidadExistente == null)
        {
            return BadRequest(new { message = $"Extremidad con Id {dto.ExtremidadId} no encontrada." });
        }

        // Crear la entidad real a partir del DTO
        var consentimiento = new ConsentimientoInformadoReal
        {
            FechaGeneracion = dto.FechaGeneracion,
            Estado = dto.Estado,
            Observacion = dto.Observacion,
            LateralidadId = dto.LateralidadId,
            ExtremidadId = dto.ExtremidadId,
            ProcedimientoId = dto.ProcedimientoId,
            PacienteId = dto.PacienteId
        };

        // Guardar en la base de datos
        _context.CONSENTIMIENTO_INFORMADO.Add(consentimiento);
        await _context.SaveChangesAsync();

        // Devolver solo el ID (sin CreatedAtAction para simplificar)
        return Ok(consentimiento.Id);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ConsentimientoInformadoReal>> GetConsentimientoInformado(int id)
    {
        var consentimiento = await _context.CONSENTIMIENTO_INFORMADO.FindAsync(id);
        if (consentimiento == null)
        {
            return NotFound();
        }
        return consentimiento;
    }
}