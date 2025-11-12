// En proyecto_hospital_version_1/Services/ConsentimientoInformadoService.cs
using System.Net.Http;
using System.Net.Http.Json; // Asegúrate de tener este using
using System.Threading.Tasks;
using ApiEntities = Hospital.Api.Data.Entities; // Mantenemos el alias

public class ConsentimientoInformadoService : IConsentimientoInformadoService
{
    private readonly HttpClient _httpClient;

    public ConsentimientoInformadoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // ¡CAMBIAR AQUÍ! Espera ConsentimientoInformadoReal
    public async Task<int> CrearConsentimientoAsync(ApiEntities.ConsentimientoInformadoReal consentimiento)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/ConsentimientoInformado", consentimiento);

            if (response.IsSuccessStatusCode)
            {
                // ✅ Leer directamente un entero, no un objeto
                var id = await response.Content.ReadFromJsonAsync<int>();
                Console.WriteLine($"[ConsentimientoService] ID recibido desde API: {id}");
                return id;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[ConsentimientoService] Error HTTP: {response.StatusCode}. Detalles: {errorContent}");
                return 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ConsentimientoService] Excepción: {ex.Message}");
            throw;
        }
    }
}