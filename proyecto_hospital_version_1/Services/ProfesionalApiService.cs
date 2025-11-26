using Hospital.Api.DTOs;
using System.Net.Http.Json;

namespace proyecto_hospital_version_1.Services;

public interface IProfesionalApiService
{
    Task<List<ProfesionalDto>?> GetProfesionalesAsync();
    Task<ProfesionalDto?> BuscarPorRutAsync(string rut);
}

public class ProfesionalApiService : IProfesionalApiService
{
    private readonly HttpClient _httpClient;
    private const string BASE_URL = "https://localhost:7032/api/Profesional";

    public ProfesionalApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Obtiene todos los profesionales
    /// </summary>
    public async Task<List<ProfesionalDto>?> GetProfesionalesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync(BASE_URL);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<ProfesionalDto>>();
            }

            Console.WriteLine($"Error obteniendo profesionales: {response.StatusCode}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Excepción en GetProfesionalesAsync: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Busca un profesional por su RUT
    /// </summary>
    /// <param name="rut">RUT del profesional (sin puntos ni guión)</param>
    public async Task<ProfesionalDto?> BuscarPorRutAsync(string rut)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(rut))
            {
                Console.WriteLine("RUT vacío o nulo");
                return null;
            }

            // Limpiar el RUT (remover puntos, guiones, espacios)
            var rutLimpio = rut.Replace(".", "").Replace("-", "").Trim();

            var response = await _httpClient.GetAsync($"{BASE_URL}/buscar-por-rut/{rutLimpio}");
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ProfesionalDto>();
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Console.WriteLine($"No se encontró profesional con RUT: {rut}");
                return null;
            }

            Console.WriteLine($"Error buscando profesional: {response.StatusCode}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Excepción en BuscarPorRutAsync: {ex.Message}");
            return null;
        }
    }
}
