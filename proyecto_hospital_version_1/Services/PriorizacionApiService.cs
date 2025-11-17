using System.Net.Http.Json;



namespace proyecto_hospital_version_1.Services
{
    public class PriorizacionApiService
    {
        private readonly HttpClient _http;
        public PriorizacionApiService(HttpClient http) => _http = http;

        // === DTOs que devuelve tu backend ===
        public class SolicitudMedicoDto
        {
            public int Id { get; set; }
            public string? NombrePaciente { get; set; }
            public string? Rut { get; set; }
            public string? Diagnostico { get; set; }
            public string? Procedimiento { get; set; }
            public string? Estado { get; set; }
            public int? Prioridad { get; set; }
            public DateTime FechaCreacion { get; set; }
        }

        public class SolicitudDetalleDto
        {
            public int Id { get; set; }
            public string? NombrePaciente { get; set; }
            public string? Rut { get; set; }
            public string? Diagnostico { get; set; }
            public string? Procedimiento { get; set; }
            public string? Procedencia { get; set; }
            public bool EsGes { get; set; }
            public string? Estado { get; set; }
            public int? Prioridad { get; set; }
            public DateTime FechaCreacion { get; set; }
            public DateTime? FechaPriorizacion { get; set; }
            public decimal? Peso { get; set; }
            public decimal? Talla { get; set; }
            public decimal? IMC { get; set; }
            public int? TiempoEstimado { get; set; }
            public bool EvaluacionAnestesica { get; set; }
            public bool EvaluacionTransfusion { get; set; }

            // para el error pi
            public string Comorbilidades { get; set; } = string.Empty;

        }

        public class GuardarPriorizacionRequest
        {
            public int SolicitudId { get; set; }
            public int CriterioPriorizacionId { get; set; }
            public string? Justificacion { get; set; }
            public DateTime FechaPriorizacion { get; set; }
        }

        // === Endpoints ===
        public Task<List<SolicitudMedicoDto>?> GetPendientesAsync() =>
            _http.GetFromJsonAsync<List<SolicitudMedicoDto>>("api/Priorizacion/pendientes");

        public Task<List<SolicitudMedicoDto>?> GetPriorizadasAsync() =>
            _http.GetFromJsonAsync<List<SolicitudMedicoDto>>("api/Priorizacion/priorizadas");

        public Task<SolicitudDetalleDto?> GetDetalleAsync(int id) =>
            _http.GetFromJsonAsync<SolicitudDetalleDto>($"api/Priorizacion/detalle/{id}");

        public async Task<bool> GuardarAsync(GuardarPriorizacionRequest body)
        {
            var res = await _http.PostAsJsonAsync("api/Priorizacion/guardar", body);
            return res.IsSuccessStatusCode;
        }
    }
}
