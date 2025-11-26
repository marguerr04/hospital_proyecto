using Hospital.Api.Data.Services;
using Hospital.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Hospital.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditoriaPriorizacionController : ControllerBase
    {
        private readonly IAuditoriaPriorizacionService _service;

        public AuditoriaPriorizacionController(IAuditoriaPriorizacionService service)
        {
            _service = service;
        }

        /// <summary>
        /// Obtiene el historial de auditoría de priorizaciones con paginación
        /// </summary>
        /// <param name="pageNumber">Número de página (por defecto 1)</param>
        /// <param name="pageSize">Cantidad de registros por página (por defecto 20)</param>
        /// <returns>Lista de registros de auditoría</returns>
        [HttpGet]
        public async Task<IActionResult> GetHistorialAuditoria([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            var auditoria = await _service.GetHistorialAuditoriaAsync(pageNumber, pageSize);
            var totalRegistros = await _service.GetTotalRegistrosAsync();

            var response = new
            {
                Data = auditoria,
                PaginaActual = pageNumber,
                TamañoPagina = pageSize,
                TotalRegistros = totalRegistros,
                TotalPaginas = (totalRegistros + pageSize - 1) / pageSize
            };

            return Ok(response);
        }

        /// <summary>
        /// Obtiene el total de registros de auditoría
        /// </summary>
        /// <returns>Cantidad total de registros</returns>
        [HttpGet("total")]
        public async Task<IActionResult> GetTotalRegistros()
        {
            var total = await _service.GetTotalRegistrosAsync();
            return Ok(new { Total = total });
        }
    }
}
