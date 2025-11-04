using System.Net.Http.Json;

namespace proyecto_hospital_version_1.Data
{
    public class DashboardService
    {
        private readonly HttpClient _http;
        private const string BaseUrl = "http://localhost:5227"; // Cambiado a http

        public DashboardService(HttpClient http)
        {
            _http = http;
        }

        public async Task<int> ObtenerPercentil75Async()
        {
            try
            {
                var result = await _http.GetFromJsonAsync<int>("api/dashboard/percentil75");
                return result; // Removido el operador ??
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerPercentil75Async: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> ObtenerReduccionAsync()
        {
            try
            {
                var result = await _http.GetFromJsonAsync<int>("api/dashboard/reduccion");
                return result; // Removido el operador ??
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerReduccionAsync: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> ObtenerPendientesAsync()
        {
            try
            {
                var result = await _http.GetFromJsonAsync<int>("api/dashboard/pendientes");
                return result; // Removido el operador ??
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerPendientesAsync: {ex.Message}");
                return 0;
            }
        }

        public async Task<Dictionary<string, double>> ObtenerPorcentajeContactoAsync()
        {
            try
            {
                var result = await _http.GetFromJsonAsync<Dictionary<string, double>>("api/dashboard/contactabilidad");
                return result ?? new Dictionary<string, double>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerPorcentajeContactoAsync: {ex.Message}");
                return new Dictionary<string, double>();
            }
        }

        public async Task<Dictionary<string, int>> ObtenerProcedimientosPorTipoAsync()
        {
            try
            {
                var result = await _http.GetFromJsonAsync<Dictionary<string, int>>("api/dashboard/procedimientos");
                return result ?? new Dictionary<string, int>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerProcedimientosPorTipoAsync: {ex.Message}");
                return new Dictionary<string, int>();
            }
        }
    }
}