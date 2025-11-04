using System.Collections.Generic;
using System.Threading.Tasks;

namespace proyecto_hospital_version_1.Services
{
    public interface IEspecialidadService
    {
        Task<List<EspecialidadDto>> GetEspecialidadesAsync();
        Task<List<EspecialidadDto>> BuscarEspecialidadesAsync(string texto);
    }
}
