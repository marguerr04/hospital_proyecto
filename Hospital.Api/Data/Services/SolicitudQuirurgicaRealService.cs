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
                //  Crear Consentimiento Informado
                var consentimiento = new ConsentimientoInformadoReal
                {
                    FechaGeneracion = DateTime.Now,
                    Estado = true,
                    LateralidadId = 1, // reemplazar si tienes variable
                    ExtremidadId = 1,
                    ProcedimientoId = 1, // idem
                    PacienteId = pacienteId,
                    Observacion = comentarios
                };
                _context.CONSENTIMIENTO_INFORMADO.Add(consentimiento);
                await _context.SaveChangesAsync();

                //  Crear Solicitud Quirúrgica Real
                var solicitud = new SolicitudQuirurgicaReal
                {
                    ConsentimientoId = consentimiento.Id,
                    DiagnosticoId = 1, // si tienes ID de diagnóstico, reemplazar
                    ProcedenciaId = 1,
                    TipoPrestacionId = 1,
                    FechaCreacion = DateTime.Now,
                    ValidacionGES = esGes,
                    ValidacionDuplicado = false
                };
                _context.SOLICITUD_QUIRURGICA.Add(solicitud);
                await _context.SaveChangesAsync();

                //  Crear Detalle Paciente
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

                // 4️⃣ Crear Detalle Clínico
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
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error al crear solicitud: {ex.Message}");
                return false;
            }
        }
    }
}
