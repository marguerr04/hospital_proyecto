using System.Net.Http.Json;
using Hospital.Api.Data.DTOs;

namespace proyecto_hospital_version_1.Services
{
    public class DiagnosticoService : IDiagnosticoService
    {
        private readonly HttpClient _httpClient;

        public DiagnosticoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<DiagnosticoDto>> GetDiagnosticosAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<List<DiagnosticoDto>>("api/diagnostico");
            return result ?? new List<DiagnosticoDto>();
        }

        public async Task<List<DiagnosticoDto>> BuscarDiagnosticosAsync(string texto)
        {
            var result = await _httpClient.GetFromJsonAsync<List<DiagnosticoDto>>($"api/diagnostico/buscar?texto={texto}");
            return result ?? new List<DiagnosticoDto>();
        }
    }
}
