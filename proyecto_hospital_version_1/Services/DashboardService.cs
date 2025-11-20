using System.Net.Http.Json;
using Hospital.Api.DTOs;
namespace proyecto_hospital_version_1.Services
{
    public class DashboardService
    {
        private readonly HttpClient _http;
        private const string BaseUrl = "http://localhost:5227"; // tu API

        public DashboardService(HttpClient http)
        {
            _http = http;
        }

        public async Task<int> ObtenerPercentil75Async()
        {
            return await _http.GetFromJsonAsync<int>("api/dashboard/percentil75");
        }

        public async Task<int> ObtenerReduccionAsync()
        {
            return await _http.GetFromJsonAsync<int>("api/dashboard/reduccion");
        }

        public async Task<int> ObtenerPendientesAsync()
        {
            return await _http.GetFromJsonAsync<int>("api/dashboard/pendientes");
        }

        public async Task<Dictionary<string, double>> ObtenerPorcentajeContactoAsync()
        {
            return await _http.GetFromJsonAsync<Dictionary<string, double>>("api/dashboard/contactabilidad")
                   ?? new Dictionary<string, double>();
        }

        public async Task<Dictionary<string, int>> ObtenerProcedimientosPorTipoAsync()
        {
            return await _http.GetFromJsonAsync<Dictionary<string, int>>("api/dashboard/procedimientos")
                   ?? new Dictionary<string, int>();
        }
    }
}
