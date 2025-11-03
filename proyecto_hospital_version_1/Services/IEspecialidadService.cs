using proyecto_hospital_version_1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace proyecto_hospital_version_1.Services
{
    public interface IEspecialidadHospitalService
    {
        Task<List<EspecialidadHospital>> GetEspecialidadesAsync();
        Task<List<string>> GetDiagnosticosSugeridosAsync(); 

    }
}