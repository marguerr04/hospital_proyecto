using Hospital.Api.DTOs;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace proyecto_hospital_version_1.Services
{
    public interface IAuditoriaPriorizacionApiService
    {
        Task<AuditoriaResponse?> GetHistorialAuditoriaAsync(int pageNumber = 1, int pageSize = 20);
        Task<int> GetTotalRegistrosAsync();
    }

    public class AuditoriaPriorizacionApiService : IAuditoriaPriorizacionApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:7032/api/AuditoriaPriorizacion";

        public AuditoriaPriorizacionApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AuditoriaResponse?> GetHistorialAuditoriaAsync(int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<AuditoriaResponse>(
                    $"{BaseUrl}?pageNumber={pageNumber}&pageSize={pageSize}");
                
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo historial de auditoría: {ex.Message}");
                return null;
            }
        }

        public async Task<int> GetTotalRegistrosAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<TotalResponse>($"{BaseUrl}/total");
                return response?.Total ?? 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo total de registros: {ex.Message}");
                return 0;
            }
        }
    }

    public class AuditoriaResponse
    {
        public List<AuditoriaPriorizacionDto> Data { get; set; } = new();
        public int PaginaActual { get; set; }
        public int TamañoPagina { get; set; }
        public int TotalRegistros { get; set; }
        public int TotalPaginas { get; set; }
    }

    public class TotalResponse
    {
        public int Total { get; set; }
    }
}
