using Hospital.Api.DTOs;

namespace proyecto_hospital_version_1.Services
{
    public interface IEspecialidadHospital
    {
        Task<List<EspecialidadDto>> GetEspecialidadesAsync();
    }
}