using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Api.Data.DTOs;   // <- DTO REAL DE LA API

namespace proyecto_hospital_version_1.Services
{
    public interface IProcedimientoService
    {
        Task<List<ProcedimientoDto>> GetProcedimientosAsync();
        Task<List<ProcedimientoDto>> BuscarProcedimientosAsync(string texto);
    }
}
