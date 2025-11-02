using proyecto_hospital_version_1.Data._Legacy;

namespace proyecto_hospital_version_1.Services
{
    public interface ISolicitudQuirurgicaService
    {
        Task<SolicitudQuirurgica> CrearSolicitudAsync(SolicitudQuirurgica nuevaSolicitud);

        Task<List<SolicitudQuirurgica>> ObtenerTodasLasSolicitudesAsync();


        Task<SolicitudQuirurgica> ObtenerSolicitudPorIdAsync(int id);

    }
}