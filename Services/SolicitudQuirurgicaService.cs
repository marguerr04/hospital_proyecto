using proyecto_hospital_version_1.Data;     // Para el DbContext
using proyecto_hospital_version_1.Models; // Para los modelos
using Microsoft.EntityFrameworkCore;
using proyecto_hospital_version_1.Data.Hospital;

namespace proyecto_hospital_version_1.Services
{
    public class SolicitudQuirurgicaService : ISolicitudQuirurgicaService
    {
        private readonly HospitalDbContext _context;

        // Inyeccion del DbContext 
        public SolicitudQuirurgicaService(HospitalDbContext context)
        {
            _context = context;
        }


        public async Task<SolicitudQuirurgica> CrearSolicitudAsync(SolicitudQuirurgica nuevaSolicitud)
        {
            try
            {
                // Vvalores por defecto al crear para que no sean nulos
                nuevaSolicitud.FechaCreacion = DateTime.Now;
                nuevaSolicitud.Estado = "Pendiente"; 
                nuevaSolicitud.Prioridad = 0; 
                nuevaSolicitud.CreadoPor = "medico_demo"; // El medico

                // Agregamos la solicitud al DbContext y se gardan en la bd
                _context.SolicitudesQuirurgicas.Add(nuevaSolicitud);
                await _context.SaveChangesAsync();

                return nuevaSolicitud; 
            }
            catch (Exception ex)
            {
                // Si  no se genera la soliocitud por alguna razon. 
                Console.WriteLine($"Error al guardar solicitud: {ex.Message}");
                throw;
            }
        }

        // Método para el Dashboard (Paso 4-5)
        public async Task<List<SolicitudQuirurgica>> ObtenerTodasLasSolicitudesAsync()
        {
            return await _context.SolicitudesQuirurgicas
                             .Include(s => s.Paciente) // Incluye los datos del Paciente
                             .AsNoTracking()
                             .ToListAsync();
        }
    }
}