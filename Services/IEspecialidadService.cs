using proyecto_hospital_version_1.Models;

namespace proyecto_hospital_version_1.Services
{
    public interface IEspecialidadService
    {
        Task<IEnumerable<Especialidad>> GetEspecialidadesAsync();
    }
}