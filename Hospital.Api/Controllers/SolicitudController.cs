using Microsoft.AspNetCore.Mvc;
using Hospital.Api.Data.Services;
using Hospital.Api.DTOs;

namespace Hospital.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SolicitudController : ControllerBase
    {
        private readonly SolicitudQuirurgicaRealService _solicitudService;

        public SolicitudController(SolicitudQuirurgicaRealService solicitudService)
        {
            _solicitudService = solicitudService;
        }

        // ✅ POST: api/solicitud/crear
        [HttpPost("crear")]
        public async Task<IActionResult> CrearSolicitud([FromBody] SolicitudCrearDto dto)
        {
            if (dto == null)
                return BadRequest(new { mensaje = "❌ El cuerpo de la solicitud está vacío." });

            try
            {
                var exito = await _solicitudService.CrearSolicitudAsync(
                    dto.PacienteId,
                    dto.ConsentimientoId,
                    dto.DiagnosticoPrincipal,
                    dto.ProcedimientoPrincipal,
                    dto.Procedencia,
                    dto.Peso,
                    dto.Talla,
                    dto.IMC,
                    dto.TiempoEstimado,
                    dto.EvaluacionAnestesica,
                    dto.EvaluacionTransfusion,
                    dto.EsGes,
                    dto.Comentarios,
                    dto.EspecialidadOrigen,
                    dto.EspecialidadDestino,
                    dto.Lateralidad,
                    dto.Extremidad
                );

                if (exito)
                    return Ok(new { mensaje = "✅ Solicitud quirúrgica creada correctamente." });

                return StatusCode(500, new { mensaje = "❌ Error al crear la solicitud quirúrgica." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "⚠️ Error interno del servidor", detalle = ex.Message });
            }
        }

        // ✅ NUEVO: POST api/solicitud/crear-y-devolver-id
        [HttpPost("crear-y-devolver-id")]
        public async Task<ActionResult<int>> CrearSolicitudYDevolverId([FromBody] SolicitudCrearDto dto)
        {
            if (dto == null)
                return BadRequest(0);

            var id = await _solicitudService.CrearSolicitudYDevolverIdAsync(
                dto.PacienteId,
                dto.ConsentimientoId,
                dto.DiagnosticoPrincipal,
                dto.ProcedimientoPrincipal,
                dto.Procedencia,
                dto.Peso,
                dto.Talla,
                dto.IMC,
                dto.TiempoEstimado,
                dto.EvaluacionAnestesica,
                dto.EvaluacionTransfusion,
                dto.EsGes,
                dto.Comentarios,
                dto.EspecialidadOrigen,
                dto.EspecialidadDestino,
                dto.Lateralidad,
                dto.Extremidad
            );

            if (id <= 0) return StatusCode(500, 0);
            return Ok(id);
        }

        // ✅ GET api/solicitud/medico/{idMedico}
        [HttpGet("medico/{idMedico}")]
        public async Task<IActionResult> ObtenerSolicitudesPorMedico(int idMedico)
        {
            try
            {
                var solicitudes = await _solicitudService.ObtenerSolicitudesPorMedicoAsync(idMedico);
                return Ok(solicitudes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "⚠️ Error al obtener solicitudes del médico", detalle = ex.Message });
            }
        }

        [HttpGet("paciente/{rut}")]
        public async Task<IActionResult> ObtenerSolicitudesPorPaciente(string rut)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(rut))
                    return BadRequest(new { mensaje = "El RUT del paciente es requerido" });

                var solicitudes = await _solicitudService.ObtenerSolicitudesPorPacienteAsync(rut);
                return Ok(solicitudes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener solicitudes del paciente", detalle = ex.Message });
            }
        }
    }
}