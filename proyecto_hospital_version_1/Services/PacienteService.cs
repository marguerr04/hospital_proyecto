using Microsoft.EntityFrameworkCore;
using proyecto_hospital_version_1.Data._Legacy;
using System.Net.Http.Json;

namespace proyecto_hospital_version_1.Services
{
    public class PacienteService : IPacienteService
    {
        private readonly HospitalDbContextLegacy _context;
        private readonly HttpClient _http;

        // ✅ Constructor actualizado: Inyecta tanto DbContext (legacy) como HttpClient (API)
        public PacienteService(HospitalDbContextLegacy context, HttpClient http)
        {
            _context = context;
            _http = http;
        }

        public async Task<PacienteHospital?> BuscarPacientePorRutAsync(string rut, string dv)
        {
            var cleanRut = rut.Trim();
            var cleanDv = dv.Trim().ToUpper();

            return await _context.Pacientes
                .Include(p => p.Ubicaciones)
                .FirstOrDefaultAsync(p => p.rut == cleanRut && p.dv.ToUpper() == cleanDv);
        }

        public async Task<List<dynamic>> BuscarPacientesApiAsync(string? texto = null, string? rut = null, string? dv = null)
        {
            try
            {
                // Construir query params
                var queryParams = new List<string>();
                if (!string.IsNullOrEmpty(texto))
                    queryParams.Add($"texto={Uri.EscapeDataString(texto)}");
                if (!string.IsNullOrEmpty(rut))
                    queryParams.Add($"rut={Uri.EscapeDataString(rut)}");
                if (!string.IsNullOrEmpty(dv))
                    queryParams.Add($"dv={Uri.EscapeDataString(dv)}");

                var query = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
                var url = $"api/Paciente/buscar{query}";

                Console.WriteLine($"[PacienteService] Llamando a API: {url}");

                var resultado = await _http.GetFromJsonAsync<List<dynamic>>(url);

                Console.WriteLine($"[PacienteService] Resultados obtenidos: {resultado?.Count ?? 0}");

                return resultado ?? new List<dynamic>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PacienteService] Error al buscar pacientes en API: {ex.Message}");
                return new List<dynamic>();
            }
        }

        public async Task<dynamic?> ObtenerPacientePorIdApiAsync(int id)
        {
            try
            {
                Console.WriteLine($"[PacienteService] Obteniendo paciente ID {id} desde API");

                var paciente = await _http.GetFromJsonAsync<dynamic>($"api/Paciente/{id}");

                return paciente;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PacienteService] Error al obtener paciente {id}: {ex.Message}");
                return null;
            }
        }
    }
}