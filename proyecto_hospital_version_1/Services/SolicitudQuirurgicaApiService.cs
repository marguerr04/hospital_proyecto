using Hospital.Api.DTOs;              // DTOs compartidos con la API
using System.Net.Http.Json;

namespace proyecto_hospital_version_1.Services
{
    // Contrato que usarás en tus componentes Blazor
    public interface ISolicitudQuirurgicaApiService
    {
        Task<bool> CrearSolicitudAsync(SolicitudCrearDto solicitudDto);
        Task<int> CrearSolicitudYDevolverIdAsync(SolicitudCrearDto solicitudDto);
        Task<List<SolicitudMedicoDto>> ObtenerSolicitudesPorMedicoAsync(int idMedico);
        Task<List<SolicitudRecienteDto>> GetSolicitudesRecientesAsync();
    }

    // Implementación que SOLO habla con la API vía HttpClient
    public class SolicitudQuirurgicaApiService : ISolicitudQuirurgicaApiService
    {
        private readonly HttpClient _http;

        public SolicitudQuirurgicaApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<bool> CrearSolicitudAsync(SolicitudCrearDto solicitudDto)
        {
            try
            {
                Console.WriteLine($"[SolicitudService] Enviando solicitud con ConsentimientoId: {solicitudDto.ConsentimientoId}");
                Console.WriteLine($"[SolicitudService] Diagnóstico: {solicitudDto.DiagnosticoPrincipal}");
                Console.WriteLine($"[SolicitudService] EspecialidadDestino: {solicitudDto.EspecialidadDestino}");

                var response = await _http.PostAsJsonAsync("api/Solicitud/crear", solicitudDto);

                Console.WriteLine($"[SolicitudService] Respuesta HTTP: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("✅ Solicitud enviada correctamente a la API.");
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Error HTTP {response.StatusCode}: {errorContent}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Error al conectar con la API: {ex.Message}");
                Console.WriteLine($"💥 StackTrace: {ex.StackTrace}");
                return false;
            }
        }

        public async Task<int> CrearSolicitudYDevolverIdAsync(SolicitudCrearDto solicitudDto)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/Solicitud/crear-y-devolver-id", solicitudDto);
                if (!response.IsSuccessStatusCode) return 0;
                var id = await response.Content.ReadFromJsonAsync<int>();
                return id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Error al crear y obtener ID: {ex.Message}");
                return 0;
            }
        }

        public async Task<List<SolicitudMedicoDto>> ObtenerSolicitudesPorMedicoAsync(int idMedico)
        {
            try
            {
                Console.WriteLine($"[SolicitudService] Obteniendo solicitudes del médico {idMedico}");

                var solicitudes = await _http
                    .GetFromJsonAsync<List<SolicitudMedicoDto>>($"api/Solicitud/medico/{idMedico}");

                Console.WriteLine($"[SolicitudService] Solicitudes obtenidas: {solicitudes?.Count ?? 0}");

                return solicitudes ?? new List<SolicitudMedicoDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Error al obtener solicitudes: {ex.Message}");
                return new List<SolicitudMedicoDto>();
            }
        }

        public async Task<List<SolicitudRecienteDto>> GetSolicitudesRecientesAsync()
        {
            try
            {
                Console.WriteLine("[SolicitudService] Obteniendo solicitudes recientes");

                var solicitudes = await _http
                    .GetFromJsonAsync<List<SolicitudRecienteDto>>("api/Solicitud/recientes");

                Console.WriteLine($"[SolicitudService] Solicitudes recientes obtenidas: {solicitudes?.Count ?? 0}");

                return solicitudes ?? new List<SolicitudRecienteDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Error al obtener solicitudes recientes: {ex.Message}");
                return new List<SolicitudRecienteDto>();
            }
        }
    }
}
