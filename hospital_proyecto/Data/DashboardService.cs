using System.Net.Http.Json;

namespace proyecto_hospital_version_1.Data
{
    public class DashboardService
    {
        private readonly HttpClient _http;

        public DashboardService(HttpClient http)
        {
            _http = http;
        }

        public async Task<int> ObtenerPercentil75Async()
            => await _http.GetFromJsonAsync<int>("api/dashboard/percentil75");

        public async Task<int> ObtenerReduccionAsync()
            => await _http.GetFromJsonAsync<int>("api/dashboard/reduccion");

        public async Task<int> ObtenerPendientesAsync()
            => await _http.GetFromJsonAsync<int>("api/dashboard/pendientes");

        public async Task<Dictionary<string, double>> ObtenerPorcentajeContactoAsync()
            => await _http.GetFromJsonAsync<Dictionary<string, double>>("api/dashboard/contactabilidad");

        public async Task<Dictionary<string, int>> ObtenerProcedimientosPorTipoAsync()
            => await _http.GetFromJsonAsync<Dictionary<string, int>>("api/dashboard/procedimientos");
    }
}
