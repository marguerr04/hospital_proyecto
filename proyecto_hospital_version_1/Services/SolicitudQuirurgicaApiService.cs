using System.Net.Http.Json;
using Hospital.Api.Data.DTOs;  // ✅ Para usar el DTO correcto
using proyecto_hospital_version_1.Data._Legacy;

namespace proyecto_hospital_version_1.Services
{
    public interface ISolicitudQuirurgicaApiService
    {
        // ✅ CAMBIADO: Ahora acepta SolicitudCrearDto
        Task<bool> CrearSolicitudAsync(SolicitudCrearDto solicitudDto);
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

                // ✅ Endpoint CORRECTO (respeta mayúsculas/minúsculas)
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
    }
}