using Microsoft.EntityFrameworkCore;
using proyecto_hospital_version_1.Data._Legacy;   // Para acceder al contexto y modelos del Legacy
 // (si tus modelos están ahí)


// se cambiaran temporalmente a legacy para comptabilidad con api y evitar errores de conflicto

namespace proyecto_hospital_version_1.Services
{
    public class SolicitudQuirurgicaService : ISolicitudQuirurgicaService
    {
        private readonly HospitalDbContextLegacy _context;

        public SolicitudQuirurgicaService(HospitalDbContextLegacy context)
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

        public async Task<SolicitudQuirurgica?> ObtenerSolicitudPorIdAsync(int id)
        {
            return await _context.SolicitudesQuirurgicas
                                 .Include(s => s.Paciente)
                                 .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<SolicitudQuirurgica>> ObtenerTodasLasSolicitudesAsync()
        {
            return await _context.SolicitudesQuirurgicas
                                 .Include(s => s.Paciente)
                                 .AsNoTracking()
                                 .ToListAsync();
        }
    }
}
