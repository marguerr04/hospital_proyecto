using Hospital.Api.Data;
using Hospital.Api.DTOs;
using Hospital.Api.Services;
using Microsoft.EntityFrameworkCore;
using Hospital.Api.Data.Entities;

namespace Hospital.Api.Data.Services
{
    public class SolicitudQuirurgicaRealService : ISolicitudQuirurgicaService
    {
        private readonly HospitalDbContext _context;

        public SolicitudQuirurgicaRealService(HospitalDbContext context)
        {
            _context = context;
        }

        // 🔹 MÉTODO PRINCIPAL - MANTENIDO IGUAL
        public async Task<bool> CrearSolicitudAsync(
            int pacienteId,
            int? consentimientoId,
            string diagnosticoPrincipal,
            string procedimientoPrincipal,
            string procedencia,
            decimal peso,
            decimal talla,
            decimal imc,
            int tiempoEstimado,
            bool evaluacionAnestesica,
            bool evaluacionTransfusion,
            bool esGes,
            string? comentarios,
            string? especialidadOrigen,
            string? especialidadDestino,
            string? lateralidad,
            string? extremidad)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                Console.WriteLine($"🔍 [Service] Creando solicitud - ConsentimientoId: {consentimientoId}");

                // 1️⃣ VALIDAR CONSENTIMIENTO ESPECÍFICO
                if (consentimientoId == null || consentimientoId == 0)
                {
                    Console.WriteLine("❌ No se proporcionó un ConsentimientoId válido");
                    return false;
                }

                var consentimiento = await _context.CONSENTIMIENTO_INFORMADO
                    .FirstOrDefaultAsync(c => c.Id == consentimientoId && c.PacienteId == pacienteId);

                if (consentimiento == null)
                {
                    Console.WriteLine($"❌ Consentimiento {consentimientoId} no existe para paciente {pacienteId}");
                    return false;
                }

                Console.WriteLine($"✅ Consentimiento validado - ID: {consentimiento.Id}");

                // 2️⃣ OBTENER IDs DE TABLAS RELACIONADAS
                var diagnosticoId = await _context.DIAGNOSTICO
                    .Where(d => d.Nombre == diagnosticoPrincipal)
                    .Select(d => d.Id)
                    .FirstOrDefaultAsync();

                var procedenciaId = await _context.PROCEDENCIA
                    .Where(p => p.Nombre == procedencia)
                    .Select(p => p.Id)
                    .FirstOrDefaultAsync();

                var tipoPrestacionKey = "Cirugía Urgencia"; // Valor seguro por defecto

                if (!string.IsNullOrWhiteSpace(especialidadDestino))
                {
                    // Solo cambiar si coincide con un tipo de prestación real
                    var tipoPrestacionValida = await _context.TIPO_PRESTACION
                        .Where(t => t.Nombre.ToLower() == especialidadDestino.ToLower())
                        .Select(t => t.Nombre)
                        .FirstOrDefaultAsync();

                    if (tipoPrestacionValida != null)
                        tipoPrestacionKey = tipoPrestacionValida;
                }

                // Buscar el ID final de tipo de prestación
                var tipoPrestacionId = await _context.TIPO_PRESTACION
                    .Where(t => t.Nombre == tipoPrestacionKey)
                    .Select(t => t.Id)
                    .FirstOrDefaultAsync();

                // 3️⃣ VALIDAR IDs OBTENIDOS
                if (diagnosticoId == 0)
                {
                    Console.WriteLine($"❌ Diagnóstico '{diagnosticoPrincipal}' no encontrado");
                    return false;
                }

                if (procedenciaId == 0)
                {
                    Console.WriteLine($"❌ Procedencia '{procedencia}' no encontrada");
                    return false;
                }

                if (tipoPrestacionId == 0)
                {
                    Console.WriteLine($"❌ Tipo prestación '{especialidadDestino}' no encontrado");
                    return false;
                }

                Console.WriteLine($"✅ IDs obtenidos - Diagnostico: {diagnosticoId}, Procedencia: {procedenciaId}, TipoPrestacion: {tipoPrestacionId}");

                // 4️⃣ CREAR SOLICITUD QUIRÚRGICA REAL
                var solicitud = new SolicitudQuirurgicaReal
                {
                    ConsentimientoId = consentimientoId.Value,
                    DiagnosticoId = diagnosticoId,
                    ProcedenciaId = procedenciaId,
                    TipoPrestacionId = tipoPrestacionId,
                    FechaCreacion = DateTime.Now,
                    ValidacionGES = esGes,
                    ValidacionDuplicado = false,
                    IdSIGTE = 1
                };

                _context.SOLICITUD_QUIRURGICA.Add(solicitud);
                await _context.SaveChangesAsync();

                Console.WriteLine($"✅ SolicitudReal creada - ID: {solicitud.IdSolicitud}");

                // 5️⃣ CREAR DETALLE PACIENTE
                var detallePaciente = new DetallePacienteReal
                {
                    SolicitudConsentimientoId = consentimientoId.Value,
                    SolicitudId = solicitud.IdSolicitud,
                    Peso = peso,
                    Altura = talla,
                    IMC = imc
                };

                _context.DETALLE_PACIENTE.Add(detallePaciente);

                // 6️⃣ CREAR DETALLE CLÍNICO
                var detalleClinico = new DetalleClinicoReal
                {
                    SolicitudConsentimientoId = consentimientoId.Value,
                    SolicitudId = solicitud.IdSolicitud,
                    TiempoEstimadoCirugia = tiempoEstimado,
                    EvaluacionAnestesica = evaluacionAnestesica,
                    EvaluacionTransfusion = evaluacionTransfusion
                };

                _context.DETALLE_CLINICO.Add(detalleClinico);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                Console.WriteLine($"🎉 SOLICITUD COMPLETA CREADA - ID: {solicitud.IdSolicitud}");
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"💥 ERROR: {ex.Message}");
                Console.WriteLine($"💥 StackTrace: {ex.StackTrace}");
                return false;
            }
        }

        // 🔹 MÉTODO SIMPLIFICADO 1: Solo datos básicos (CON DATOS REALES O DUMMY)
        public async Task<IEnumerable<SolicitudRecienteDto>> GetSolicitudesRecientesAsync()
        {
            try
            {
                // Intentar obtener datos REALES primero
                var solicitudesReales = await _context.SOLICITUD_QUIRURGICA
                    .Include(s => s.Consentimiento)
                    .ThenInclude(c => c.Paciente)
                    .Include(s => s.Consentimiento)
                    .ThenInclude(c => c.Procedimiento)
                    .OrderByDescending(s => s.FechaCreacion)
                    .Take(10)
                    .ToListAsync();

                var resultado = new List<SolicitudRecienteDto>();

                foreach (var solicitud in solicitudesReales)
                {
                    var dto = new SolicitudRecienteDto
                    {
                        SolicitudId = solicitud.IdSolicitud,
                        PacienteNombreCompleto = $"{solicitud.Consentimiento?.Paciente?.PrimerNombre} {solicitud.Consentimiento?.Paciente?.ApellidoPaterno}",
                        PacienteRut = solicitud.Consentimiento?.Paciente?.Rut ?? "N/A",
                        Prioridad = solicitud.ValidacionGES ? "Prioritaria" : "Intermedia",
                        PrioridadCssClass = solicitud.ValidacionGES ? "bg-success" : "bg-warning text-dark",
                        EsGes = solicitud.ValidacionGES,
                        DescripcionProcedimiento = solicitud.Consentimiento?.Procedimiento?.Nombre ?? "Procedimiento no especificado",
                        FechaCreacion = solicitud.FechaCreacion,
                        TiempoTranscurrido = CalculateTimeAgo(solicitud.FechaCreacion)
                    };
                    resultado.Add(dto);
                }

                // Si no hay datos reales, usar dummy
                if (!resultado.Any())
                {
                    resultado = GetSolicitudesDummy();
                }

                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetSolicitudesRecientesAsync: {ex.Message}");
                // Fallback a datos dummy
                return GetSolicitudesDummy();
            }
        }

        // 🔹 MÉTODO SIMPLIFICADO 2: SOLO DATOS DUMMY (sin PROGRAMACION_QUIRURGICA)
        public async Task<IEnumerable<FechaProgramadaDto>> GetProximasFechasProgramadasAsync()
        {
            try
            {
                // ❌ ELIMINADA la consulta a PROGRAMACION_QUIRURGICA que no existe
                // ✅ SOLO datos dummy por ahora
                return GetFechasProgramadasDummy();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetProximasFechasProgramadasAsync: {ex.Message}");
                return GetFechasProgramadasDummy();
            }
        }

        // 🔹 MÉTODOS AUXILIARES PARA DATOS DUMMY
        private List<SolicitudRecienteDto> GetSolicitudesDummy()
        {
            return new List<SolicitudRecienteDto>
            {
                new SolicitudRecienteDto
                {
                    SolicitudId = 1,
                    PacienteNombreCompleto = "Juan Pérez González",
                    PacienteRut = "12.345.678-9",
                    Prioridad = "Prioritaria",
                    PrioridadCssClass = "bg-success",
                    EsGes = true,
                    DescripcionProcedimiento = "Cirugía de cadera",
                    FechaCreacion = DateTime.Now.AddDays(-2),
                    TiempoTranscurrido = "Hace 2 días"
                },
                new SolicitudRecienteDto
                {
                    SolicitudId = 2,
                    PacienteNombreCompleto = "María López Silva",
                    PacienteRut = "98.765.432-1",
                    Prioridad = "Intermedia",
                    PrioridadCssClass = "bg-warning text-dark",
                    EsGes = false,
                    DescripcionProcedimiento = "Artroscopía rodilla",
                    FechaCreacion = DateTime.Now.AddDays(-5),
                    TiempoTranscurrido = "Hace 5 días"
                }
            };
        }

        private List<FechaProgramadaDto> GetFechasProgramadasDummy()
        {
            return new List<FechaProgramadaDto>
            {
                new FechaProgramadaDto
                {
                    ProgramacionId = 1,
                    FechaProgramada = DateTime.Today.AddDays(1),
                    FechaProgramadaFormateada = DateTime.Today.AddDays(1).ToString("dd MMMM yyyy"),
                    PacienteNombreCompleto = "Paciente Demo 1",
                    DescripcionProcedimiento = "Procedimiento de prueba",
                    EsGes = true,
                    HoraProgramada = TimeSpan.FromHours(9),
                    Pabellon = "Pabellón Central"
                },
                new FechaProgramadaDto
                {
                    ProgramacionId = 2,
                    FechaProgramada = DateTime.Today.AddDays(3),
                    FechaProgramadaFormateada = DateTime.Today.AddDays(3).ToString("dd MMMM yyyy"),
                    PacienteNombreCompleto = "Paciente Demo 2",
                    DescripcionProcedimiento = "Cirugía programada",
                    EsGes = false,
                    HoraProgramada = TimeSpan.FromHours(11),
                    Pabellon = "Pabellón Norte"
                }
            };
        }

        // 🔹 FUNCIÓN AUXILIAR
        private string CalculateTimeAgo(DateTime date)
        {
            var diff = DateTime.Now - date;
            if (diff.TotalDays < 1) return $"Hace {(int)diff.TotalHours} horas";
            if (diff.TotalDays < 30) return $"Hace {(int)diff.TotalDays} días";
            return date.ToString("dd/MM/yyyy");
        }
    }
}