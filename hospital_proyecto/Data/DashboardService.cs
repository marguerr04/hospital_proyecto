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

        public async Task<Dictionary<string, int>> ObtenerProcedimientosPorTipoAsync(DateTime? desde = null, DateTime? hasta = null)
        {
            var query = "";
            if (desde.HasValue) query += $"desde={desde.Value:yyyy-MM-dd}&";
            if (hasta.HasValue) query += $"hasta={hasta.Value:yyyy-MM-dd}";
            
            var result = await _http.GetFromJsonAsync<Dictionary<string, int>>($"{BaseUrl}/api/dashboard/procedimientos?{query}");
            return result ?? new Dictionary<string, int>();
        }

        public async Task<Dictionary<string, double>> ObtenerContactabilidadAsync(DateTime? desde = null, DateTime? hasta = null)
        {
            var query = "";
            if (desde.HasValue) query += $"desde={desde.Value:yyyy-MM-dd}&";
            if (hasta.HasValue) query += $"hasta={hasta.Value:yyyy-MM-dd}";

            var result = await _http.GetFromJsonAsync<Dictionary<string, double>>($"{BaseUrl}/api/dashboard/contactabilidad?{query}");
            return result ?? new Dictionary<string, double>();
        }

        public async Task<List<EvolucionPercentil>> ObtenerEvolucionPercentilAsync(DateTime? desde = null, DateTime? hasta = null)
        {
            var query = "";
            if (desde.HasValue) query += $"desde={desde.Value:yyyy-MM-dd}&";
            if (hasta.HasValue) query += $"hasta={hasta.Value:yyyy-MM-dd}";

            var result = await _http.GetFromJsonAsync<List<EvolucionPercentil>>($"{BaseUrl}/api/dashboard/evolucion-percentil?{query}");
            return result ?? new List<EvolucionPercentil>();
        }

        public async Task<Dictionary<string, int>> ObtenerCausalEgresoAsync(DateTime? desde = null, DateTime? hasta = null)
        {
            var query = "";
            if (desde.HasValue) query += $"desde={desde.Value:yyyy-MM-dd}&";
            if (hasta.HasValue) query += $"hasta={hasta.Value:yyyy-MM-dd}";

            var result = await _http.GetFromJsonAsync<Dictionary<string, int>>($"{BaseUrl}/api/dashboard/causal-egreso?{query}");
            return result ?? new Dictionary<string, int>();
        }
    }

    public class EvolucionPercentil
    {
        public string Mes { get; set; } = "";
        public int Valor { get; set; }
    }
}