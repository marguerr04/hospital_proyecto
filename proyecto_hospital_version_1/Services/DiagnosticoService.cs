using System.Net.Http.Json;
using proyecto_hospital_version_1.Data._Legacy;

namespace proyecto_hospital_version_1.Services
{
    public class DiagnosticoService : IDiagnosticoService
    {
        private readonly HttpClient _httpClient;

        public DiagnosticoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Diagnostico>> GetDiagnosticosAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<List<Diagnostico>>("api/diagnostico");
            return result ?? new List<Diagnostico>();
        }

        public async Task<List<Diagnostico>> BuscarDiagnosticosAsync(string texto)
        {
            var result = await _httpClient.GetFromJsonAsync<List<Diagnostico>>($"api/diagnostico/buscar?texto={texto}");
            return result ?? new List<Diagnostico>();
        }
    }
}
