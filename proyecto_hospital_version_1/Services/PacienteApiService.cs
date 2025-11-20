using Hospital.Api.Data.DTOs;
using Hospital.Api.DTOs;
using System.Net.Http.Json;

namespace proyecto_hospital_version_1.Services
{
    public class PacienteApiService
    {
        private readonly HttpClient _http;

        public PacienteApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<PacienteDto?> ObtenerPacientePorIdAsync(int id)
        {
            try
            {
                return await _http.GetFromJsonAsync<PacienteDto>($"api/Paciente/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<PacienteDto>> BuscarPacientesAsync(string? texto, string? rut, string? dv)
        {
            var query = new List<string>();

            if (!string.IsNullOrWhiteSpace(texto)) query.Add($"texto={texto}");
            if (!string.IsNullOrWhiteSpace(rut)) query.Add($"rut={rut}");
            if (!string.IsNullOrWhiteSpace(dv)) query.Add($"dv={dv}");

            var url = "api/Paciente/buscar";

            if (query.Any())
                url += "?" + string.Join("&", query);

            try
            {
                var result = await _http.GetFromJsonAsync<List<PacienteDto>>(url);
                return result ?? new List<PacienteDto>();
            }
            catch
            {
                return new List<PacienteDto>();
            }
        }
    }
}
