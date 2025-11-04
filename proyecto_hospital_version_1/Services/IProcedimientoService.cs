using System.Threading.Tasks;

namespace proyecto_hospital_version_1.Services
{
    public interface IProcedimientoService
    {
        Task<List<ProcedimientoDto>> GetProcedimientosAsync();
        Task<List<ProcedimientoDto>> BuscarProcedimientosAsync(string texto);
    }
}
