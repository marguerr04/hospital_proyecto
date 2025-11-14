using Hospital.Api.Data.DTOs;  // ✅ AGREGAR ESTA LÍNEA
using proyecto_hospital_version_1.Data._Legacy;
using System.Net.Http.Json;

namespace proyecto_hospital_version_1.Services
{
    public interface ISolicitudQuirurgicaApiService
    {
        Task<bool> CrearSolicitudAsync(SolicitudCrearDto solicitudDto);
        Task<List<SolicitudMedicoDto>> ObtenerSolicitudesPorMedicoAsync(int idMedico);
    }

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

        public async Task<List<SolicitudMedicoDto>> ObtenerSolicitudesPorMedicoAsync(int idMedico)
        {
            try
            {
                Console.WriteLine($"[SolicitudService] Obteniendo solicitudes del médico {idMedico}");

                var solicitudes = await _http.GetFromJsonAsync<List<SolicitudMedicoDto>>($"api/Solicitud/medico/{idMedico}");

                Console.WriteLine($"[SolicitudService] Solicitudes obtenidas: {solicitudes?.Count ?? 0}");

                return solicitudes ?? new List<SolicitudMedicoDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Error al obtener solicitudes: {ex.Message}");
                return new List<SolicitudMedicoDto>();
            }
        }
    }
}