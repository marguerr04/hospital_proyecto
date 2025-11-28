using System.Net.Http.Json;
using Hospital.Api.DTOs;

namespace proyecto_hospital_version_1.Services
{
    public class DashboardService
    {
        private readonly HttpClient _http;

        public DashboardService(HttpClient http)
        {
            _http = http;
        }

        // ✅ Endpoints básicos sin filtros
        public async Task<int> ObtenerPercentil75Async()
        {
            try
            {
                var result = await _http.GetFromJsonAsync<int>("api/dashboard/percentil75");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerPercentil75Async: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> ObtenerPercentil75FiltradoAsync(
            DateTime? desde = null,
            DateTime? hasta = null,
            string? sexo = null,
            bool? ges = null)
        {
            try
            {
                var query = BuildQueryString(desde, hasta, sexo, ges);
                var url = $"api/dashboard/percentil75-filtrado{query}";
                var result = await _http.GetFromJsonAsync<int>(url);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerPercentil75FiltradoAsync: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> ObtenerReduccionAsync()
        {
            try
            {
                var result = await _http.GetFromJsonAsync<int>("api/dashboard/reduccion");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerReduccionAsync: {ex.Message}");
                return 25; // Valor por defecto
            }
        }

        public async Task<object> ObtenerReduccionRealAsync(
            DateTime? desde = null,
            DateTime? hasta = null)
        {
            try
            {
                var query = BuildQueryString(desde, hasta, null, null);
                var url = $"api/dashboard/reduccion-real{query}";
                var result = await _http.GetFromJsonAsync<object>(url);
                return result ?? new { PorcentajeReduccion = 0, MesActual = 0, MesAnterior = 0 };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerReduccionRealAsync: {ex.Message}");
                return new { PorcentajeReduccion = 0, MesActual = 0, MesAnterior = 0 };
            }
        }

        public async Task<int> ObtenerPendientesAsync()
        {
            try
            {
                var result = await _http.GetFromJsonAsync<int>("api/dashboard/pendientes");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerPendientesAsync: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> ObtenerPendientesFiltradoAsync(
            DateTime? desde = null,
            DateTime? hasta = null,
            string? sexo = null,
            bool? ges = null)
        {
            try
            {
                var query = BuildQueryString(desde, hasta, sexo, ges);
                var url = $"api/dashboard/pendientes-filtrado{query}";
                var result = await _http.GetFromJsonAsync<int>(url);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerPendientesFiltradoAsync: {ex.Message}");
                return 0;
            }
        }

        // ✅ Endpoints con filtros opcionales

        public async Task<Dictionary<string, int>> ObtenerProcedimientosPorTipoAsync(
            DateTime? desde = null,
            DateTime? hasta = null,
            string? sexo = null,
            bool? ges = null)
        {
            try
            {
                var query = BuildQueryString(desde, hasta, sexo, ges);
                var url = $"api/dashboard/procedimientos{query}";
                var result = await _http.GetFromJsonAsync<Dictionary<string, int>>(url);
                return result ?? new Dictionary<string, int>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerProcedimientosPorTipoAsync: {ex.Message}");
                return new Dictionary<string, int>();
            }
        }

        public async Task<Dictionary<string, double>> ObtenerContactabilidadAsync(
            DateTime? desde = null,
            DateTime? hasta = null,
            string? sexo = null,
            bool? ges = null)
        {
            try
            {
                var query = BuildQueryString(desde, hasta, sexo, ges);
                var url = $"api/dashboard/contactabilidad{query}";
                var result = await _http.GetFromJsonAsync<Dictionary<string, double>>(url);
                return result ?? new Dictionary<string, double>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerContactabilidadAsync: {ex.Message}");
                return new Dictionary<string, double>();
            }
        }

        public async Task<List<EvolucionPercentilDto>> ObtenerEvolucionPercentilAsync(
            DateTime? desde = null,
            DateTime? hasta = null,
            string? sexo = null,
            bool? ges = null)
        {
            try
            {
                var query = BuildQueryString(desde, hasta, sexo, ges);
                var url = $"api/dashboard/evolucion-percentil{query}";
                var result = await _http.GetFromJsonAsync<List<EvolucionPercentilDto>>(url);
                return result ?? new List<EvolucionPercentilDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerEvolucionPercentilAsync: {ex.Message}");
                return new List<EvolucionPercentilDto>();
            }
        }

        public async Task<Dictionary<string, int>> ObtenerCausalEgresoAsync(
            DateTime? desde = null,
            DateTime? hasta = null,
            string? sexo = null,
            bool? ges = null)
        {
            try
            {
                var query = BuildQueryString(desde, hasta, sexo, ges);
                var url = $"api/dashboard/egresos/por-causal{query}";
                
                // La API devuelve un array de objetos con Causal, Total, Porcentaje
                var result = await _http.GetFromJsonAsync<List<CausalEgresoDto>>(url);
                
                if (result == null || !result.Any())
                    return new Dictionary<string, int>();

                // Convertir a Dictionary<string, int>
                return result.ToDictionary(x => x.Causal, x => x.Total);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerCausalEgresoAsync: {ex.Message}");
                return new Dictionary<string, int>();
            }
        }

        public async Task<List<EvolucionProcedimientoDto>> ObtenerEvolucionProcedimientoAsync(
            string procedimiento,
            DateTime? desde = null,
            DateTime? hasta = null,
            string? sexo = null,
            bool? ges = null)
        {
            try
            {
                var query = BuildQueryString(desde, hasta, sexo, ges);
                // asegurar separador correcto entre query y procedimiento
                var separator = string.IsNullOrEmpty(query) ? "?" : "&";
                var url = $"api/dashboard/evolucion-procedimiento{query}{separator}procedimiento={System.Net.WebUtility.UrlEncode(procedimiento)}";
                var result = await _http.GetFromJsonAsync<List<EvolucionProcedimientoDto>>(url);
                return result ?? new List<EvolucionProcedimientoDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerEvolucionProcedimientoAsync: {ex.Message}");
                return new List<EvolucionProcedimientoDto>();
            }
        }

        // ✅ Helper para construir query strings
        private string BuildQueryString(DateTime? desde, DateTime? hasta, string? sexo, bool? ges)
        {
            var parameters = new List<string>();

            if (desde.HasValue)
                parameters.Add($"desde={desde.Value:yyyy-MM-dd}");

            if (hasta.HasValue)
                parameters.Add($"hasta={hasta.Value:yyyy-MM-dd}");

            if (!string.IsNullOrEmpty(sexo) && sexo != "Todos")
                parameters.Add($"sexo={sexo}");

            if (ges.HasValue)
                parameters.Add($"ges={ges.Value.ToString().ToLower()}");

            return parameters.Count > 0 ? "?" + string.Join("&", parameters) : string.Empty;
        }
    }

    // ✅ DTO para la respuesta de Causal de Egreso
    public class CausalEgresoDto
    {
        public string Causal { get; set; } = string.Empty;
        public int Total { get; set; }
        public double Porcentaje { get; set; }
    }

    // DTO para la evolución por procedimiento
    public class EvolucionProcedimientoDto
    {
        public string Fecha { get; set; } = string.Empty; // yyyy-MM-dd
        public int Cantidad { get; set; }
    }
}
