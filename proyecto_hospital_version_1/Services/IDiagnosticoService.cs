using proyecto_hospital_version_1.Data._Legacy;

namespace proyecto_hospital_version_1.Services
{
    public interface IDiagnosticoService
    {
        Task<List<Diagnostico>> GetDiagnosticosAsync();
        Task<List<Diagnostico>> BuscarDiagnosticosAsync(string texto);
    }
}