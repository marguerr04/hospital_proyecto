using proyecto_hospital_version_1.Models; // Asegúrate de que el namespace sea correcto

namespace proyecto_hospital_version_1.Services
{
    public interface IEspecialidadHospital
    {
        Task<List<EspecialidadHospital>> GetEspecialidadesAsync();
    }
}