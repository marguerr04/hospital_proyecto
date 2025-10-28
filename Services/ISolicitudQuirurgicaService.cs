using proyecto_hospital_version_1.Models; 

namespace proyecto_hospital_version_1.Services
{
    public interface ISolicitudQuirurgicaService
    {
        Task<SolicitudQuirurgica> CrearSolicitudAsync(SolicitudQuirurgica nuevaSolicitud);

        Task<List<SolicitudQuirurgica>> ObtenerTodasLasSolicitudesAsync();
    }
}