// Hospital.Api/Services/ISolicitudQuirurgicaDashboardService.cs
using Hospital.Api.DTOs; // Referencia a tus DTOs

namespace Hospital.Api.Data.Services // ¡Namespace que coincide con SolicitudQuirurgicaRealService!
{
    public interface ISolicitudQuirurgicaDashboardService
    {
        Task<IEnumerable<SolicitudRecienteDto>> GetSolicitudesRecientesAsync();
        Task<IEnumerable<FechaProgramadaDto>> GetProximasFechasProgramadasAsync();
    }
}