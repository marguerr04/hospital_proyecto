using Microsoft.AspNetCore.Mvc;
using Hospital.Api.Data.DTOs;
using Hospital.Api.Data.Services;

namespace Hospital.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PriorizacionController : ControllerBase
    {
        private readonly ISolicitudQuirurgicaService _solicitudService;

        public PriorizacionController(ISolicitudQuirurgicaService solicitudService)
        {
            _solicitudService = solicitudService;
        }

        [HttpGet("pendientes")]
        public async Task<IActionResult> ObtenerSolicitudesPendientes()
        {
            try
            {
                var solicitudes = await _solicitudService.ObtenerSolicitudesPendientesAsync();
                return Ok(solicitudes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpGet("priorizadas")]
        public async Task<IActionResult> ObtenerSolicitudesPriorizadas()
        {
            try
            {
                var solicitudes = await _solicitudService.ObtenerSolicitudesPriorizadasAsync();
                return Ok(solicitudes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpGet("detalle/{id}")]
        public async Task<IActionResult> ObtenerDetalleSolicitud(int id)
        {
            try
            {
                var detalle = await _solicitudService.ObtenerSolicitudDetalleAsync(id);
                if (detalle == null)
                    return NotFound(new { mensaje = "Solicitud no encontrada" });

                return Ok(detalle);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpPost("guardar")]
        public async Task<IActionResult> GuardarPriorizacion([FromBody] PriorizacionDto priorizacion)
        {
            try
            {
                if (priorizacion == null)
                    return BadRequest(new { mensaje = "Datos de priorización inválidos" });

                var exito = await _solicitudService.GuardarPriorizacionAsync(priorizacion);
                if (exito)
                    return Ok(new { mensaje = "Priorización guardada correctamente" });

                return BadRequest(new { mensaje = "Error al guardar priorización" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", error = ex.Message });
            }
        }







        [HttpGet("diagnostico")]
        public async Task<IActionResult> DiagnosticoBaseDatos()
        {
            try
            {
                // Contar solicitudes totales
                var totalSolicitudes = await _solicitudService.ObtenerSolicitudesPorMedicoAsync(1);
                var pendientes = await _solicitudService.ObtenerSolicitudesPendientesAsync();
                var priorizadas = await _solicitudService.ObtenerSolicitudesPriorizadasAsync();

                return Ok(new
                {
                    TotalSolicitudes = totalSolicitudes.Count(),
                    Pendientes = pendientes.Count(),
                    Priorizadas = priorizadas.Count(),
                    TodasLasSolicitudes = totalSolicitudes,
                    SolicitudesPendientes = pendientes,
                    SolicitudesPriorizadas = priorizadas
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error en diagnóstico",
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }










    }
}