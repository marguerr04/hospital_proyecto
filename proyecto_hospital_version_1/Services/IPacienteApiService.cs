using Hospital.Api.DTOs;

namespace proyecto_hospital_version_1.Services
{
    public interface IPacienteApiService
    {
        Task<PacienteDto?> ObtenerPacientePorIdAsync(int id);
        Task<List<PacienteDto>> BuscarPacientesAsync(string? texto = null, string? rut = null, string? dv = null);
    }
}
