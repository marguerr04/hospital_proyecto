using System.Net.Http.Json;
using proyecto_hospital_version_1.Data._Legacy;  // para usar tu modelo existente

namespace proyecto_hospital_version_1.Services
{
    public interface ISolicitudQuirurgicaApiService
    {
        Task<bool> CrearSolicitudAsync(SolicitudQuirurgica solicitud);
    }

    public class SolicitudQuirurgicaApiService : ISolicitudQuirurgicaApiService
    {
        private readonly HttpClient _http;

        public SolicitudQuirurgicaApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<bool> CrearSolicitudAsync(SolicitudQuirurgica solicitud)
        {
            try
            {
                // Enviamos el mismo modelo al endpoint de la API
                var response = await _http.PostAsJsonAsync("api/solicitud/crear", solicitud);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Solicitud enviada correctamente a la API.");
                    return true;
                }
                else
                {
                    var msg = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($" Error al crear solicitud: {msg}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error al conectar con la API: {ex.Message}");
                return false;
            }
        }
    }
}
