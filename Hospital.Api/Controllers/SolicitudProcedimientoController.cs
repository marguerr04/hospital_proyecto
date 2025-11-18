using Hospital.Api.Data;
using Hospital.Api.Data.DTOs;
using Hospital.Api.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Api.Controllers
{
    [ApiController]
    [Route("api/solicitud/{solicitudId:int}/procedimientos")]
    public class SolicitudProcedimientoController : ControllerBase
    {
        private readonly HospitalDbContext _context;
        public SolicitudProcedimientoController(HospitalDbContext context)
        {
            _context = context;
        }

        // GET: api/solicitud/{id}/procedimientos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SolicitudProcedimientoDto>>> Listar(int solicitudId, [FromQuery] int? consentimientoId)
        {
            // Buscar la solicitud y validar consentimiento si se envía
            var solicitud = await _context.SOLICITUD_QUIRURGICA
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.IdSolicitud == solicitudId);
            if (solicitud == null)
                return NotFound(new { mensaje = $"Solicitud {solicitudId} no encontrada" });

            int consentId = consentimientoId ?? solicitud.ConsentimientoId;

            var items = await _context.SOLICITUD_QUIRURGICA_PROCEDIMIENTO
                .Where(sp => sp.SolicitudId == solicitudId && sp.SolicitudConsentimientoId == consentId)
                .Include(sp => sp.Procedimiento)
                .OrderBy(sp => sp.Orden)
                .Select(sp => new SolicitudProcedimientoDto
                {
                    SolicitudId = sp.SolicitudId,
                    ConsentimientoId = sp.SolicitudConsentimientoId,
                    ProcedimientoId = sp.ProcedimientoId,
                    Nombre = sp.Procedimiento!.Nombre,
                    Codigo = sp.Procedimiento.Codigo,
                    EsPrincipal = sp.EsPrincipal,
                    Orden = sp.Orden,
                    Observaciones = sp.Observaciones
                })
                .ToListAsync();

            return Ok(items);
        }

        // POST: api/solicitud/{id}/procedimientos
        [HttpPost]
        public async Task<IActionResult> Agregar(int solicitudId, [FromQuery] int? consentimientoId, [FromBody] SolicitudProcedimientoCrearDto dto)
        {
            if (dto == null) return BadRequest(new { mensaje = "Body requerido" });

            var solicitud = await _context.SOLICITUD_QUIRURGICA
                .FirstOrDefaultAsync(s => s.IdSolicitud == solicitudId);
            if (solicitud == null)
                return NotFound(new { mensaje = $"Solicitud {solicitudId} no encontrada" });

            int consentId = consentimientoId ?? solicitud.ConsentimientoId;

            // Validar procedimiento existente
            var procExiste = await _context.PROCEDIMIENTO.AnyAsync(p => p.Id == dto.ProcedimientoId);
            if (!procExiste) return BadRequest(new { mensaje = $"Procedimiento {dto.ProcedimientoId} no existe" });

            var yaExiste = await _context.SOLICITUD_QUIRURGICA_PROCEDIMIENTO
                .AnyAsync(sp => sp.SolicitudId == solicitudId && sp.SolicitudConsentimientoId == consentId && sp.ProcedimientoId == dto.ProcedimientoId);
            if (yaExiste) return Conflict(new { mensaje = "Ya existe ese procedimiento en la solicitud." });

            // Garantizar solo un principal
            if (dto.EsPrincipal)
            {
                await _context.SOLICITUD_QUIRURGICA_PROCEDIMIENTO
                    .Where(sp => sp.SolicitudId == solicitudId && sp.SolicitudConsentimientoId == consentId && sp.EsPrincipal)
                    .ExecuteUpdateAsync(s => s.SetProperty(p => p.EsPrincipal, false));
            }

            var entity = new SolicitudProcedimiento
            {
                SolicitudId = solicitudId,
                SolicitudConsentimientoId = consentId,
                ProcedimientoId = dto.ProcedimientoId,
                EsPrincipal = dto.EsPrincipal,
                Orden = dto.Orden ?? 1,
                Observaciones = dto.Observaciones,
                FechaAsignacion = DateTime.UtcNow
            };

            _context.SOLICITUD_QUIRURGICA_PROCEDIMIENTO.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Listar), new { solicitudId, consentimientoId = consentId }, new { mensaje = "Agregado" });
        }

        // DELETE: api/solicitud/{id}/procedimientos/{procedimientoId}
        [HttpDelete("{procedimientoId:int}")]
        public async Task<IActionResult> Eliminar(int solicitudId, int procedimientoId, [FromQuery] int? consentimientoId)
        {
            var solicitud = await _context.SOLICITUD_QUIRURGICA
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.IdSolicitud == solicitudId);
            if (solicitud == null)
                return NotFound(new { mensaje = $"Solicitud {solicitudId} no encontrada" });

            int consentId = consentimientoId ?? solicitud.ConsentimientoId;

            var entity = await _context.SOLICITUD_QUIRURGICA_PROCEDIMIENTO
                .FirstOrDefaultAsync(sp => sp.SolicitudId == solicitudId && sp.SolicitudConsentimientoId == consentId && sp.ProcedimientoId == procedimientoId);
            if (entity == null)
                return NotFound(new { mensaje = $"No existe vínculo con procedimiento {procedimientoId}" });

            _context.SOLICITUD_QUIRURGICA_PROCEDIMIENTO.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
