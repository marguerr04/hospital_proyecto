using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Api.DTOs;   // <- DTO  API

namespace proyecto_hospital_version_1.Services
{
    public interface IProcedimientoService
    {
        Task<List<ProcedimientoDto>> GetProcedimientosAsync();
        Task<List<ProcedimientoDto>> BuscarProcedimientosAsync(string texto);
    }
}
