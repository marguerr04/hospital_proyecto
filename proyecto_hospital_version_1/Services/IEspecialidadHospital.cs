using proyecto_hospital_version_1.Data._Legacy;

namespace proyecto_hospital_version_1.Services
{
    public interface IEspecialidadHospital
    {
        Task<List<EspecialidadHospital>> GetEspecialidadesAsync();
    }
}