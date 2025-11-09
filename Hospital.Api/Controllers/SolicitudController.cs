using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Hospital.Api.Data.Services;

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

        // POST: api/solicitud/crear
        [HttpPost("crear")]
        public async Task<IActionResult> CrearSolicitud([FromBody] SolicitudCrearDto dto)
        {
            if (dto == null)
                return BadRequest("Datos vacíos.");

            try
            {
                var exito = await _solicitudService.CrearSolicitudAsync(
                    dto.PacienteId,
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
                    return Ok(new { mensaje = " Solicitud creada correctamente" });
                else
                    return StatusCode(500, new { mensaje = "❌ Error al crear solicitud" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }
    }

    // DTO simple para recibir datos del frontend
    public class SolicitudCrearDto
    {
        public int PacienteId { get; set; }
        public string DiagnosticoPrincipal { get; set; } = string.Empty;
        public string ProcedimientoPrincipal { get; set; } = string.Empty;
        public string Procedencia { get; set; } = "Ambulatorio";
        public decimal Peso { get; set; }
        public decimal Talla { get; set; }
        public decimal IMC { get; set; }
        public int TiempoEstimado { get; set; }
        public bool EvaluacionAnestesica { get; set; }
        public bool EvaluacionTransfusion { get; set; }
        public bool EsGes { get; set; }
        public string? Comentarios { get; set; }
        public string? EspecialidadOrigen { get; set; }
        public string? EspecialidadDestino { get; set; }

        public string? Lateralidad { get; set; }
        public string? Extremidad { get; set; }
    }
}
