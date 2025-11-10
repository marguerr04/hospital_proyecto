using Hospital.Api.Data;
using Hospital.Api.DTOs;
using Hospital.Api.Services;
using Microsoft.EntityFrameworkCore;
using proyecto_hospital_version_1.Data.Entities;

namespace Hospital.Api.Data.Services
{
    public class SolicitudQuirurgicaRealService : ISolicitudQuirurgicaService
    {
        private readonly HospitalDbContext _context;

        public SolicitudQuirurgicaRealService(HospitalDbContext context)
        {
            _context = context;
        }

        // 🔹 MÉTODO EXISTENTE (lo mantienes igual)
        public async Task<bool> CrearSolicitudAsync(
            int pacienteId,
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
                // Tu código existente aquí...
                var diagnosticoId = await _context.DIAGNOSTICO
                    .Where(d => d.Nombre == diagnosticoPrincipal)
                    .Select(d => d.Id)
                    .FirstOrDefaultAsync();

                var procedimientoId = await _context.PROCEDIMIENTO
                    .Where(p => p.Nombre == procedimientoPrincipal)
                    .Select(p => p.Id)
                    .FirstOrDefaultAsync();

                var procedenciaId = await _context.PROCEDENCIA
                    .Where(p => p.Nombre == procedencia)
                    .Select(p => p.Id)
                    .FirstOrDefaultAsync();

                var tipoPrestacionId = await _context.TIPO_PRESTACION
                    .Where(t => t.Nombre == especialidadDestino)
                    .Select(t => t.Id)
                    .FirstOrDefaultAsync();

                var lateralidadId = await _context.LATERALIDAD
                   .Where(l => l.Nombre == lateralidad)
                   .Select(l => l.Id)
                   .FirstOrDefaultAsync();

                var extremidadId = await _context.EXTREMIDAD
                    .Where(e => e.Nombre == extremidad)
                    .Select(e => e.Id)
                    .FirstOrDefaultAsync();

                // Validar si existen
                if (diagnosticoId == 0 || procedimientoId == 0 || procedenciaId == 0)
                {
                    Console.WriteLine(" Error: No se encontraron IDs válidos en las tablas relacionadas.");
                    return false;
                }

                // 2️⃣ Crear Consentimiento Informado
                var consentimiento = new ConsentimientoInformadoReal
                {
                    FechaGeneracion = DateTime.Now,
                    Estado = true,
                    LateralidadId = lateralidadId,
                    ExtremidadId = extremidadId,
                    ProcedimientoId = procedimientoId,
                    PacienteId = pacienteId,
                    Observacion = comentarios
                };
                _context.CONSENTIMIENTO_INFORMADO.Add(consentimiento);
                await _context.SaveChangesAsync();

                // 3️⃣ Crear Solicitud Quirúrgica
                var solicitud = new SolicitudQuirurgicaReal
                {
                    ConsentimientoId = consentimiento.Id,
                    DiagnosticoId = diagnosticoId,
                    ProcedenciaId = procedenciaId,
                    TipoPrestacionId = tipoPrestacionId,
                    FechaCreacion = DateTime.Now,
                    ValidacionGES = esGes,
                    ValidacionDuplicado = false
                };
                _context.SOLICITUD_QUIRURGICA.Add(solicitud);
                await _context.SaveChangesAsync();

                // 4️⃣ Crear Detalle Paciente
                var detallePaciente = new DetallePacienteReal
                {
                    SolicitudConsentimientoId = consentimiento.Id,
                    SolicitudId = solicitud.IdSolicitud,
                    Peso = peso,
                    Altura = talla,
                    IMC = imc
                };
                _context.DETALLE_PACIENTE.Add(detallePaciente);
                await _context.SaveChangesAsync();

                // 5️⃣ Crear Detalle Clínico
                var detalleClinico = new DetalleClinicoReal
                {
                    SolicitudConsentimientoId = consentimiento.Id,
                    SolicitudId = solicitud.IdSolicitud,
                    TiempoEstimadoCirugia = tiempoEstimado,
                    EvaluacionAnestesica = evaluacionAnestesica,
                    EvaluacionTransfusion = evaluacionTransfusion
                };
                _context.DETALLE_CLINICO.Add(detalleClinico);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                Console.WriteLine("Solicitud creada correctamente con datos reales del frontend.");
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($" Error al crear solicitud: {ex.Message}");
                return false;
            }
        }

        // 🔹 MÉTODO SIMPLIFICADO 1: Solo datos básicos
        public async Task<IEnumerable<SolicitudRecienteDto>> GetSolicitudesRecientesAsync()
        {
            try
            {
                // Método ULTRA-SIMPLE que SÍ compila
                var resultado = new List<SolicitudRecienteDto>
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

                return await Task.FromResult(resultado);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetSolicitudesRecientesAsync: {ex.Message}");
                return new List<SolicitudRecienteDto>();
            }
        }

        // 🔹 MÉTODO SIMPLIFICADO 2: Datos dummy para pruebas
        public async Task<IEnumerable<FechaProgramadaDto>> GetProximasFechasProgramadasAsync()
        {
            try
            {
                // Por ahora devolvemos datos dummy para que Blazor funcione
                var fechasDummy = new List<FechaProgramadaDto>
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

                return await Task.FromResult(fechasDummy);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetProximasFechasProgramadasAsync: {ex.Message}");
                return new List<FechaProgramadaDto>();
            }
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