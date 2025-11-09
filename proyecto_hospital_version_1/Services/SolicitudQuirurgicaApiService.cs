using System.Net.Http.Json;
using System.Text.Json;

namespace proyecto_hospital_version_1.Services
{
    public interface ISolicitudQuirurgicaApiService
    {
        Task<bool> CrearSolicitudAsync(SolicitudCrearDto dto);
    }

    public class SolicitudQuirurgicaApiService : ISolicitudQuirurgicaApiService
    {
        private readonly HttpClient _http;

        public SolicitudQuirurgicaApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<bool> CrearSolicitudAsync(SolicitudCrearDto dto)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/solicitud/crear", dto);
                if (response.IsSuccessStatusCode)
                    return true;

                var msg = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error API: {msg}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error al conectar con la API: {ex.Message}");
                return false;
            }
        }
    }

    // Este DTO debe coincidir con el que usa tu API
    public class SolicitudCrearDto
    {
        public int PacienteId { get; set; }
        public string DiagnosticoPrincipal { get; set; } = string.Empty;
        public string ProcedimientoPrincipal { get; set; } = string.Empty;
        public string Procedencia { get; set; } = "Ambulatorio";
        public decimal Peso { get; set; }
        public decimal Talla { get; set; }
        public decimal IMC { get; set; }
        public int TiempoEstimado { get; set; }
        public bool EvaluacionAnestesica { get; set; }
        public bool EvaluacionTransfusion { get; set; }
        public bool EsGes { get; set; }
        public string? Comentarios { get; set; }
        public string? EspecialidadOrigen { get; set; }
        public string? EspecialidadDestino { get; set; }
    }
}
