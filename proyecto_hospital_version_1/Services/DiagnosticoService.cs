using System.Net.Http.Json;
using Hospital.Api.DTOs;

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
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<DiagnosticoDto>>("api/diagnostico");
                return result ?? new List<DiagnosticoDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo diagnósticos: {ex.Message}");
                return new List<DiagnosticoDto>();
            }
        }

        public async Task<List<DiagnosticoDto>> GetDiagnosticosGesAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<DiagnosticoDto>>("api/diagnostico/ges");
                return result ?? new List<DiagnosticoDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo diagnósticos GES: {ex.Message}");
                return new List<DiagnosticoDto>();
            }
        }

        public async Task<List<DiagnosticoDto>> BuscarDiagnosticosAsync(string texto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(texto))
                    return await GetDiagnosticosAsync();

                var result = await _httpClient.GetFromJsonAsync<List<DiagnosticoDto>>($"api/diagnostico/buscar?texto={Uri.EscapeDataString(texto)}");
                return result ?? new List<DiagnosticoDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error buscando diagnósticos: {ex.Message}");
                return new List<DiagnosticoDto>();
            }
        }
    }
}