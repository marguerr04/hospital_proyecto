using System.Net.Http.Json;
using Hospital.Api.Data.DTOs;
namespace proyecto_hospital_version_1.Services
{
    public class ProcedimientoService : IProcedimientoService
    {
        private readonly HttpClient _http;

        public ProcedimientoService(HttpClient http)
        {
            _http = http;
        }

        // Obtener lista completa
        public async Task<List<ProcedimientoDto>> GetProcedimientosAsync()
        {
            var result = await _http.GetFromJsonAsync<List<ProcedimientoDto>>("api/procedimiento");
            return result ?? new List<ProcedimientoDto>();
        }

        // uscar por texto (nombre o código)
        public async Task<List<ProcedimientoDto>> BuscarProcedimientosAsync(string texto)
        {
            var url = $"api/procedimiento/buscar?texto={Uri.EscapeDataString(texto)}";
            var result = await _http.GetFromJsonAsync<List<ProcedimientoDto>>(url);
            return result ?? new List<ProcedimientoDto>();
        }
    }


}
