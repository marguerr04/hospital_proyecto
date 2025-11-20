using Hospital.Api.DTOs;

namespace Hospital.Api.Services
{
    public interface IDiagnosticoService
    {
        Task<List<DiagnosticoDto>> GetDiagnosticosAsync();
        Task<List<DiagnosticoDto>> GetDiagnosticosGesAsync();
        Task<List<DiagnosticoDto>> BuscarDiagnosticosAsync(string? texto = null);
        Task<DiagnosticoDto?> GetDiagnosticoByIdAsync(int id);
    }
}