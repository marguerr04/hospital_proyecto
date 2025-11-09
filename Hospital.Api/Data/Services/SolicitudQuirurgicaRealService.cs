using Hospital.Api.Data;
using Microsoft.EntityFrameworkCore;
using proyecto_hospital_version_1.Data.Entities;
using Hospital.Api.Data;
namespace  Hospital.Api.Data.Services
{
    public class SolicitudQuirurgicaRealService
    {
        private readonly HospitalDbContext _context;

        public SolicitudQuirurgicaRealService(HospitalDbContext context)
        {
            _context = context;
        }

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
    string? especialidadDestino)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1️⃣ Buscar IDs reales según los textos enviados por el frontend

                // Buscar diagnóstico
                var diagnosticoId = await _context.DIAGNOSTICO
                    .Where(d => d.Nombre == diagnosticoPrincipal)
                    .Select(d => d.Id)
                    .FirstOrDefaultAsync();

                // Buscar procedimiento
                var procedimientoId = await _context.PROCEDIMIENTO
                    .Where(p => p.Nombre == procedimientoPrincipal)
                    .Select(p => p.Id)
                    .FirstOrDefaultAsync();

                // Buscar procedencia
                var procedenciaId = await _context.PROCEDENCIA
                    .Where(p => p.Nombre == procedencia)
                    .Select(p => p.Id)
                    .FirstOrDefaultAsync();

                // Buscar tipo de prestación (solo si tienes esa tabla)
                var tipoPrestacionId = await _context.TIPO_PRESTACION
                    .Where(t => t.Nombre == especialidadDestino)
                    .Select(t => t.Id)
                    .FirstOrDefaultAsync();

                // Validar si existen (evita insertar basura)
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
                    LateralidadId = 1, // se puede conectar con tu _lateralidadSeleccionada más adelante
                    ExtremidadId = 1,
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
    }
}
