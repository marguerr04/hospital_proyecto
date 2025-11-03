using proyecto_hospital_version_1.Data._Legacy; 
using System.Threading.Tasks;
using proyecto_hospital_version_1.Models;

namespace proyecto_hospital_version_1.Services
{
    public interface IPacienteService
    {
        Task<PacienteHospital?> BuscarPacientePorRutAsync(string rut, string dv);
    }
}