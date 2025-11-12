using Hospital.Api.DTOs;

namespace Hospital.Api.Services
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
    }
}
