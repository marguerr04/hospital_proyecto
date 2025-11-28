using Hospital.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly HospitalDbContext _context;

        public DashboardController(HospitalDbContext context)
        {
            _context = context;
        }

        //  Percentil 75 basado SOLO en SOLICITUD_QUIRURGICA
        [HttpGet("percentil75")]
        public async Task<ActionResult<double>> GetPercentil75()
        {
            var lista = await _context.SOLICITUD_QUIRURGICA // ahora debe pasar po CI para saber idpaciente
                .Include(s => s.Consentimiento)
                .GroupBy(s => s.Consentimiento.PacienteId)
                .Select(g => g.Count())
                .ToListAsync();

            if (!lista.Any())
                return Ok(0);

            lista.Sort();
            int index = (int)Math.Ceiling(0.75 * lista.Count) - 1;
            index = Math.Max(0, index);
            double percentil75 = lista[index];

            return Ok(percentil75);
        }

        //  Valor fijo de ejemplo (meta del hospital)
        [HttpGet("reduccion")]
        public Task<ActionResult<int>> GetReduccion()
        {
            return Task.FromResult<ActionResult<int>>(Ok(25));
        }

        //  Nuevo endpoint para reducción real de solicitudes
        [HttpGet("reduccion-real")]
        public async Task<ActionResult<object>> GetReduccionReal(
            [FromQuery] DateTime? desde,
            [FromQuery] DateTime? hasta)
        {
            try
            {
                var fechaHasta = hasta ?? DateTime.Today;
                var fechaDesde = desde ?? fechaHasta.AddMonths(-1);

                var mesActualInicio = new DateTime(fechaHasta.Year, fechaHasta.Month, 1);
                var mesActualFin = mesActualInicio.AddMonths(1).AddDays(-1);

                var mesAnteriorInicio = mesActualInicio.AddMonths(-1);
                var mesAnteriorFin = mesActualInicio.AddDays(-1);

                var solicitudesMesActual = await _context.SOLICITUD_QUIRURGICA
                    .Where(s => s.FechaCreacion >= mesActualInicio && s.FechaCreacion <= mesActualFin)
                    .CountAsync();

                var solicitudesMesAnterior = await _context.SOLICITUD_QUIRURGICA
                    .Where(s => s.FechaCreacion >= mesAnteriorInicio && s.FechaCreacion <= mesAnteriorFin)
                    .CountAsync();

                int porcentajeReduccion = 0;
                if (solicitudesMesAnterior > 0)
                {
                    var diferencia = solicitudesMesAnterior - solicitudesMesActual;
                    porcentajeReduccion = (int)Math.Round((diferencia * 100.0) / solicitudesMesAnterior);
                }

                return Ok(new
                {
                    PorcentajeReduccion = porcentajeReduccion,
                    MesActual = solicitudesMesActual,
                    MesAnterior = solicitudesMesAnterior
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetReduccionReal: {ex.Message}");
                return Ok(new
                {
                    PorcentajeReduccion = 0,
                    MesActual = 0,
                    MesAnterior = 0
                });
            }
        }

        //  Total de solicitudes quirurgicas registradas
        [HttpGet("pendientes")]
        public async Task<ActionResult<int>> GetPendientes()
        {
            var count = await _context.SOLICITUD_QUIRURGICA.CountAsync();
            return Ok(count);
        }

        //  CONTACTABILIDAD CON DATOS REALES BASADOS EN CONTACTO_SOLICITUD
        [HttpGet("contactabilidad")]
        public async Task<ActionResult<Dictionary<string, double>>> GetContactabilidad(
            [FromQuery] DateTime? desde,
            [FromQuery] DateTime? hasta,
            [FromQuery] string? sexo,
            [FromQuery] bool? ges)
        {
            try
            {
                DateTime desdeValue = (desde?.Date) ?? DateTime.MinValue;
                DateTime hastaExclusiveValue = (hasta?.Date.AddDays(1)) ?? DateTime.MaxValue;

                var query = _context.SOLICITUD_QUIRURGICA
                    .Include(s => s.Consentimiento)
                    .ThenInclude(c => c.Paciente)
                    .AsQueryable();

                query = query.Where(s => s.FechaCreacion >= desdeValue && s.FechaCreacion < hastaExclusiveValue);

                if (!string.IsNullOrEmpty(sexo))
                {
                    var sexoLimpio = sexo.Trim().ToUpper();
                    query = query.Where(s => s.Consentimiento != null && s.Consentimiento.Paciente != null && s.Consentimiento.Paciente.Sexo.Trim().ToUpper() == sexoLimpio);
                }

                if (ges.HasValue)
                    query = query.Where(s => s.ValidacionGES == ges.Value);

                var solicitudes = await query
                    .Select(s => new
                    {
                        SolicitudId = s.IdSolicitud,
                        TieneContacto = _context.CONTACTO_SOLICITUD
                            .Any(c => c.SOLICITUD_QUIRURGICA_idSolicitud == s.IdSolicitud &&
                                      c.fechaContacto >= desdeValue &&
                                      c.fechaContacto < hastaExclusiveValue)
                    })
                    .ToListAsync();

                var total = solicitudes.Count;
                if (total == 0)
                {
                    return Ok(new Dictionary<string, double>
                    {
                        { "Contactado", 0 },
                        { "No contactado", 0 }
                    });
                }

                var contactados = solicitudes.Count(s => s.TieneContacto);
                var noContactados = total - contactados;

                var contactabilidad = new Dictionary<string, double>
                {
                    { "Contactado", Math.Round((contactados * 100.0) / total, 1) },
                    { "No contactado", Math.Round((noContactados * 100.0) / total, 1) }
                };

                return Ok(contactabilidad);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetContactabilidad: {ex.Message}");

                return Ok(new Dictionary<string, double>
                {
                    { "Contactado", 0 },
                    { "No contactado", 0 }
                });
            }
        }

        //  PROCEDIMIENTOS CON DATOS REALES Y FILTROS
        [HttpGet("procedimientos")]
        public async Task<ActionResult<Dictionary<string, int>>> GetProcedimientosPorTipo(
            [FromQuery] DateTime? desde,
            [FromQuery] DateTime? hasta,
            [FromQuery] string? sexo,
            [FromQuery] bool? ges)
        {
            try
            {
                Console.WriteLine($"[DEBUG] GetProcedimientosPorTipo - Filtros: desde={desde}, hasta={hasta}, sexo={sexo}, ges={ges}");

                DateTime? desdeDate = desde?.Date;
                DateTime? hastaExclusive = hasta?.Date.AddDays(1);

                var query = _context.SOLICITUD_QUIRURGICA
                    .Include(s => s.Consentimiento)
                        .ThenInclude(c => c.Procedimiento)
                    .Include(s => s.Consentimiento)
                        .ThenInclude(c => c.Paciente)
                    .AsQueryable();

                if (desdeDate.HasValue)
                    query = query.Where(s => s.FechaCreacion >= desdeDate.Value);

                if (hastaExclusive.HasValue)
                    query = query.Where(s => s.FechaCreacion < hastaExclusive.Value);

                if (!string.IsNullOrEmpty(sexo))
                    query = query.Where(s => s.Consentimiento != null && 
                                           s.Consentimiento.Paciente != null && 
                                           s.Consentimiento.Paciente.Sexo.Trim().ToUpper() == sexo.Trim().ToUpper());

                if (ges.HasValue)
                    query = query.Where(s => s.ValidacionGES == ges.Value);

                var procedimientosSolicitudes = await query
                    .Where(s => s.Consentimiento != null && s.Consentimiento.Procedimiento != null)
                    .Select(s => new
                    {
                        SolicitudId = s.IdSolicitud,
                        ProcedimientoNombre = s.Consentimiento.Procedimiento.Nombre
                    })
                    .ToListAsync();

                Console.WriteLine($"[DEBUG] Solicitudes con procedimiento: {procedimientosSolicitudes.Count}");

                if (procedimientosSolicitudes.Any())
                {
                    var resultado = procedimientosSolicitudes
                        .GroupBy(x => x.ProcedimientoNombre)
                        .Select(g => new { Nombre = g.Key, Count = g.Count() })
                        .OrderByDescending(x => x.Count)
                        .Take(20)
                        .ToDictionary(x => x.Nombre ?? "Sin nombre", x => x.Count);

                    Console.WriteLine($"[DEBUG] Resultado: {resultado.Count} procedimientos únicos");
                    return Ok(resultado);
                }

                Console.WriteLine("[DEBUG] No se encontraron datos - Devolviendo diccionario vacío");
                return Ok(new Dictionary<string, int>());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetProcedimientosPorTipo: {ex.Message}");
                return Ok(new Dictionary<string, int>());
            }
        }

        // Evolución percentil
        [HttpGet("evolucion-percentil")]
        public async Task<IActionResult> GetEvolucionPercentil(
            [FromQuery] DateTime? desde,
            [FromQuery] DateTime? hasta,
                    [FromQuery] string? sexo,
            [FromQuery] bool? ges)
        {
            try
            {
                Console.WriteLine($"🔍 Filtros: desde={desde}, hasta={hasta}, sexo={sexo}, ges={ges}");

                DateTime? desdeDate = desde?.Date;
                DateTime? hastaExclusive = hasta?.Date.AddDays(1);

                var query = _context.SOLICITUD_QUIRURGICA
                    .Include(s => s.Consentimiento)
                    .ThenInclude(c => c.Paciente)
                    .AsQueryable();

                if (desdeDate.HasValue)
                    query = query.Where(s => s.FechaCreacion >= desdeDate.Value);

                if (hastaExclusive.HasValue)
                    query = query.Where(s => s.FechaCreacion < hastaExclusive.Value);

                if (!string.IsNullOrEmpty(sexo))
                {
                    var sexoLimpio = sexo.Trim().ToUpper(); // M o F
                    query = query.Where(s => s.Consentimiento != null && 
                                           s.Consentimiento.Paciente != null && 
                                           s.Consentimiento.Paciente.Sexo.Trim().ToUpper() == sexoLimpio);
                }

                if (ges.HasValue)
                    query = query.Where(s => s.ValidacionGES == ges.Value);

                Console.WriteLine($"🔍 Query construida, ejecutando...");

                var solicitudesFiltradas = await query
                    .Select(s => new
                    {
                        s.IdSolicitud,
                        s.FechaCreacion,
                        PacienteId = s.Consentimiento.PacienteId,
                        Sexo = s.Consentimiento.Paciente.Sexo,
                        EsGes = s.ValidacionGES
                    })
                    .ToListAsync();

                Console.WriteLine($"🔍 Solicitudes obtenidas: {solicitudesFiltradas.Count}");

                var datosPorMes = solicitudesFiltradas
                    .GroupBy(s => new { Year = s.FechaCreacion.Year, Month = s.FechaCreacion.Month })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        TotalSolicitudes = g.Count(),
                        SolicitudesPorPaciente = g.GroupBy(x => x.PacienteId).Select(p => p.Count()).ToList()
                    })
                    .OrderBy(x => x.Year)
                    .ThenBy(x => x.Month)
                    .ToList();

                Console.WriteLine($"🔍 Meses agrupados: {datosPorMes.Count}");

                var resultado = datosPorMes.Select(mes =>
                {
                    var lista = mes.SolicitudesPorPaciente;
                    int percentil75 = mes.TotalSolicitudes; // Fallback simple

                    if (lista.Any())
                    {
                        lista.Sort();
                        int index = (int)Math.Ceiling(0.75 * lista.Count) - 1;
                        index = Math.Max(0, index);
                        percentil75 = lista[index];
                    }

                    var fecha = new DateTime(mes.Year, mes.Month, 1);
                    return new
                    {
                        Mes = fecha.ToString("MMM", System.Globalization.CultureInfo.CreateSpecificCulture("es-ES")),
                        Valor = percentil75
                    };
                }).ToList();

                if (resultado.Any())
                {
                    Console.WriteLine($"Evolución calculada para {resultado.Count} meses con datos reales");
                    return Ok(resultado);
                }

                Console.WriteLine("Sin datos reales, devolviendo lista vacía");
                return Ok(new List<object>());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetEvolucionPercentil: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");

                return Ok(new List<object>());
            }
        }

        //  NUEVO: Endpoint auxiliar para verificar datos
        [HttpGet("estadisticas")]
        public async Task<ActionResult<object>> GetEstadisticas()
        {
            var stats = new
            {
                TotalPacientes = await _context.PACIENTE.CountAsync(),
                TotalConsentimientos = await _context.CONSENTIMIENTO_INFORMADO.CountAsync(),
                TotalSolicitudes = await _context.SOLICITUD_QUIRURGICA.CountAsync(),
                TotalProcedimientos = await _context.PROCEDIMIENTO.CountAsync(),
                ConsentimientosActivos = await _context.CONSENTIMIENTO_INFORMADO.CountAsync(c => c.Estado),
                ConsentimientosPendientes = await _context.CONSENTIMIENTO_INFORMADO.CountAsync(c => !c.Estado)
            };

            return Ok(stats);
        }

        // endpoint para los egressos de solciitud
        [HttpGet("egresos/total")]
        public async Task<ActionResult<int>> GetTotalEgresos()
        {
            var total = await _context.EGRESO_SOLICITUD.CountAsync();
            return Ok(total);
        }

        // (con % y filtros)
        [HttpGet("egresos/por-causal")]
        public async Task<ActionResult<IEnumerable<object>>> GetEgresosPorCausal(
            [FromQuery] DateTime? desde,
            [FromQuery] DateTime? hasta,
            [FromQuery] string? sexo,
            [FromQuery] bool? ges)
        {
            try
            {
                // Normalize dates
                DateTime? desdeDate = desde?.Date;
                DateTime? hastaExclusive = hasta?.Date.AddDays(1);

                // 1️⃣ Query base con navegaciones
                var query = _context.EGRESO_SOLICITUD
                    .Include(e => e.CausalSalida)
                    .AsQueryable();

                // 2️⃣ Aplicar filtro de fecha
                if (desdeDate.HasValue)
                    query = query.Where(e => e.FechaSalida >= desdeDate.Value);

                if (hastaExclusive.HasValue)
                    query = query.Where(e => e.FechaSalida < hastaExclusive.Value);

                // 3️⃣ Si hay filtros de sexo o GES, necesitamos join con solicitud
                if (!string.IsNullOrEmpty(sexo) || ges.HasValue)
                {
                    query = query.Where(e =>
                        _context.SOLICITUD_QUIRURGICA.Any(sq =>
                            sq.IdSolicitud == e.SolicitudId &&
                            (!string.IsNullOrEmpty(sexo) ? sq.Consentimiento.Paciente.Sexo.Trim().ToUpper() == sexo.Trim().ToUpper() : true) &&
                            (ges.HasValue ? sq.ValidacionGES == ges.Value : true)
                        )
                    );
                }

                var egresos = await query.ToListAsync();
                var total = egresos.Count;

                if (total == 0)
                    return Ok(Array.Empty<object>());

                var data = egresos
                    .GroupBy(e => new { e.CausalSalidaId, e.CausalSalida!.Nombre })
                    .Select(g => new
                    {
                        Causal = g.Key.Nombre,
                        Total = g.Count(),
                        Porcentaje = Math.Round(g.Count() * 100.0 / total, 1)
                    })
                    .OrderByDescending(x => x.Total)
                    .ToList();

                return Ok(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en GetEgresosPorCausal: {ex.Message}");
                return Ok(Array.Empty<object>());
            }
        }

        [HttpGet("egresos/ultimos")]
        public async Task<ActionResult<IEnumerable<object>>> GetUltimosEgresos([FromQuery] int top = 10)
        {
            top = Math.Clamp(top, 1, 100);

            var items = await _context.EGRESO_SOLICITUD
                .Include(e => e.CausalSalida)
                .OrderByDescending(e => e.FechaSalida)
                .Take(top)
                .Select(e => new
                {
                    e.Id,
                    e.FechaSalida,
                    Causal = e.CausalSalida!.Nombre,
                    e.SolicitudId,
                    e.Descripcion
                })
                .ToListAsync();

            return Ok(items);
        }
    }
}