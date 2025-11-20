using Hospital.Api.DTOs;

namespace proyecto_hospital_version_1.Services
{
    public interface IDiagnosticoService
    {
        Task<List<DiagnosticoDto>> GetDiagnosticosAsync();
        Task<List<DiagnosticoDto>> BuscarDiagnosticosAsync(string texto);
    }
}
