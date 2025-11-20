using Hospital.Api.DTOs;
using System.Threading.Tasks;

namespace Hospital.Api.Data.Services
{
    public interface ISolicitudQuirurgicaService
    {
        Task<bool> CrearSolicitudAsync(
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
            string? extremidad
        );

        Task<IEnumerable<SolicitudRecienteDto>> GetSolicitudesRecientesAsync();
        Task<IEnumerable<FechaProgramadaDto>> GetProximasFechasProgramadasAsync();



        Task<IEnumerable<SolicitudMedicoDto>> ObtenerSolicitudesPorMedicoAsync(int idMedico);


        // Para priorizacion


        Task<IEnumerable<SolicitudMedicoDto>> ObtenerSolicitudesPendientesAsync();
        Task<IEnumerable<SolicitudMedicoDto>> ObtenerSolicitudesPriorizadasAsync();
        Task<SolicitudDetalleDto?> ObtenerSolicitudDetalleAsync(int solicitudId);
        Task<bool> GuardarPriorizacionAsync(PriorizacionDto priorizacion);


    }
}
