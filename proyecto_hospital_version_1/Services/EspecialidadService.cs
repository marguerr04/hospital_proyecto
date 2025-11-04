using System.Collections.Generic;
using System;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace proyecto_hospital_version_1.Services
{
    public class EspecialidadService : IEspecialidadService
    {
        private readonly HttpClient _http;

        public EspecialidadService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<EspecialidadDto>> GetEspecialidadesAsync()
        {
            var result = await _http.GetFromJsonAsync<List<EspecialidadDto>>("api/especialidad");
            return result ?? new List<EspecialidadDto>();
        }

        public async Task<List<EspecialidadDto>> BuscarEspecialidadesAsync(string texto)
        {
            var url = $"api/especialidad/buscar?texto={Uri.EscapeDataString(texto)}";
            var result = await _http.GetFromJsonAsync<List<EspecialidadDto>>(url);
            return result ?? new List<EspecialidadDto>();
        }
    }

    public class EspecialidadDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}
