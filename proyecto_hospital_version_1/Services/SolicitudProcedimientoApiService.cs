using System.Net.Http.Json;
using Hospital.Api.Data.DTOs;

namespace proyecto_hospital_version_1.Services
{
    public interface ISolicitudProcedimientoApiService
    {
        Task<List<SolicitudProcedimientoDto>> ListarAsync(int solicitudId, int consentimientoId);
        Task<bool> AgregarAsync(int solicitudId, int consentimientoId, SolicitudProcedimientoCrearDto dto);
        Task<bool> EliminarAsync(int solicitudId, int procedimientoId, int consentimientoId);
    }

    public class SolicitudProcedimientoApiService : ISolicitudProcedimientoApiService
    {
        private readonly HttpClient _http;
        public SolicitudProcedimientoApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<SolicitudProcedimientoDto>> ListarAsync(int solicitudId, int consentimientoId)
        {
            var url = $"api/solicitud/{solicitudId}/procedimientos?consentimientoId={consentimientoId}";
            var result = await _http.GetFromJsonAsync<List<SolicitudProcedimientoDto>>(url);
            return result ?? new List<SolicitudProcedimientoDto>();
        }

        public async Task<bool> AgregarAsync(int solicitudId, int consentimientoId, SolicitudProcedimientoCrearDto dto)
        {
            var url = $"api/solicitud/{solicitudId}/procedimientos?consentimientoId={consentimientoId}";
            var resp = await _http.PostAsJsonAsync(url, dto);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> EliminarAsync(int solicitudId, int procedimientoId, int consentimientoId)
        {
            var url = $"api/solicitud/{solicitudId}/procedimientos/{procedimientoId}?consentimientoId={consentimientoId}";
            var resp = await _http.DeleteAsync(url);
            return resp.IsSuccessStatusCode;
        }
    }
}
