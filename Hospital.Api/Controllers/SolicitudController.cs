using Microsoft.AspNetCore.Mvc;
using Hospital.Api.Data.Services;
using Hospital.Api.Data.DTOs; // (opcional si mueves el DTO a esta carpeta)

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
    }

    // =============================
    // ✅ DTO que refleja el flujo real
    // =============================
    public class SolicitudCrearDto
    {
        public int PacienteId { get; set; }
        // para el id del consetnimiento
        public int ConsentimientoId { get; set; }

        // --- Datos base ---
        public string DiagnosticoPrincipal { get; set; } = string.Empty;
        public string ProcedimientoPrincipal { get; set; } = string.Empty;
        public string Procedencia { get; set; } = "Ambulatorio";

        // --- Datos clínicos ---
        public decimal Peso { get; set; }
        public decimal Talla { get; set; }
        public decimal IMC { get; set; }
        public int TiempoEstimado { get; set; }

        // --- Evaluaciones ---
        public bool EvaluacionAnestesica { get; set; }
        public bool EvaluacionTransfusion { get; set; }
        public bool EsGes { get; set; }

        // --- Contexto ---
        public string? Comentarios { get; set; }
        public string? EspecialidadOrigen { get; set; }
        public string? EspecialidadDestino { get; set; }

        // --- Campos opcionales (compatibilidad) ---
        public string? Lateralidad { get; set; }
        public string? Extremidad { get; set; }
    }
}
