using System.Net.Http.Json;
using Hospital.Api.DTOs;

namespace proyecto_hospital_version_1.Services
{
    public class EspecialidadHospitalService : IEspecialidadHospital
    {
        private readonly HttpClient _httpClient;

        public EspecialidadHospitalService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<EspecialidadDto>> GetEspecialidadesAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<EspecialidadDto>>("api/especialidad");
                return result ?? new List<EspecialidadDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo especialidades: {ex.Message}");
                return new List<EspecialidadDto>();
            }
        }
    }
}