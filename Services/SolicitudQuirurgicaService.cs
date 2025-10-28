using proyecto_hospital_version_1.Data;     // Para el DbContext
using proyecto_hospital_version_1.Models; // Para los modelos
using Microsoft.EntityFrameworkCore;
using proyecto_hospital_version_1.Data.Hospital;

namespace proyecto_hospital_version_1.Services
{
    public class SolicitudQuirurgicaService : ISolicitudQuirurgicaService
    {
        private readonly HospitalDbContext _context;

        public SolicitudQuirurgicaService(HospitalDbContext context)
        {
            _context = context;
        }

        public async Task<SolicitudQuirurgica> CrearSolicitudAsync(SolicitudQuirurgica nuevaSolicitud)
        {
            try
            {
                nuevaSolicitud.FechaCreacion = DateTime.Now;
                nuevaSolicitud.Estado = "Pendiente";
                nuevaSolicitud.Prioridad = 0;
                nuevaSolicitud.CreadoPor = "medico_demo";

                _context.SolicitudesQuirurgicas.Add(nuevaSolicitud);
                await _context.SaveChangesAsync();

                return nuevaSolicitud;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar solicitud: {ex.Message}");
                throw;
            }
        }

        // ✅ MÉTODO NUEVO/MODIFICADO - Para cargar solicitud con datos del paciente
        public async Task<SolicitudQuirurgica> ObtenerSolicitudPorIdAsync(int id)
        {
            return await _context.SolicitudesQuirurgicas
                             .Include(s => s.Paciente) // ✅ INCLUYE PACIENTE
                             .FirstOrDefaultAsync(s => s.Id == id);
        }

        // Método para el Dashboard (Paso 4-5) - YA LO TIENES
        public async Task<List<SolicitudQuirurgica>> ObtenerTodasLasSolicitudesAsync()
        {
            return await _context.SolicitudesQuirurgicas
                             .Include(s => s.Paciente)
                             .AsNoTracking()
                             .ToListAsync();
        }
    }
}