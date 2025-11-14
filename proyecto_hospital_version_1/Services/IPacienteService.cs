using proyecto_hospital_version_1.Data._Legacy;
using System.Threading.Tasks;

namespace proyecto_hospital_version_1.Services
{
    public interface IPacienteService
    {

        Task<PacienteHospital?> BuscarPacientePorRutAsync(string rut, string dv);


        Task<List<dynamic>> BuscarPacientesApiAsync(string? texto = null, string? rut = null, string? dv = null);
        Task<dynamic?> ObtenerPacientePorIdApiAsync(int id);
    }
}