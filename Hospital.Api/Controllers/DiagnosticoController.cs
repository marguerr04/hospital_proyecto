using Hospital.Api.DTOs;
using Hospital.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiagnosticoController : ControllerBase
    {
        private readonly IDiagnosticoService _diagnosticoService;

        public DiagnosticoController(IDiagnosticoService diagnosticoService)
        {
            _diagnosticoService = diagnosticoService;
        }

        // GET: api/diagnostico
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DiagnosticoDto>>> GetDiagnosticos()
        {
            var diagnosticos = await _diagnosticoService.GetDiagnosticosAsync();
            return Ok(diagnosticos);
        }

        // GET: api/diagnostico/ges
        [HttpGet("ges")]
        public async Task<ActionResult<IEnumerable<DiagnosticoDto>>> GetDiagnosticosGes()
        {
            var diagnosticos = await _diagnosticoService.GetDiagnosticosGesAsync();
            return Ok(diagnosticos);
        }

        // GET: api/diagnostico/buscar?texto=apendicitis
        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<DiagnosticoDto>>> BuscarDiagnosticos([FromQuery] string? texto = null)
        {
            var diagnosticos = await _diagnosticoService.BuscarDiagnosticosAsync(texto);
            return Ok(diagnosticos);
        }

        // GET: api/diagnostico/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<DiagnosticoDto>> GetDiagnostico(int id)
        {
            var diagnostico = await _diagnosticoService.GetDiagnosticoByIdAsync(id);
            if (diagnostico == null) return NotFound();
            return Ok(diagnostico);
        }
    }
}