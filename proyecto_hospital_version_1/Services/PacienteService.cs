using Microsoft.EntityFrameworkCore;
using proyecto_hospital_version_1.Models;
using proyecto_hospital_version_1.Data._Legacy; // El DbContext sigue en Data.Hospital

// se cambiar a dbcontestlegacy para evitar cualquier conflicto con la api


namespace proyecto_hospital_version_1.Services
{
    public class PacienteService : IPacienteService
    {
        private readonly HospitalDbContextLegacy _context;

        public PacienteService(HospitalDbContextLegacy context)
        {
            _context = context;
        }

        public async Task<PacienteHospital?> BuscarPacientePorRutAsync(string rut, string dv)
        {
            var cleanRut = rut.Trim();
            var cleanDv = dv.Trim().ToUpper();
            return await _context.Pacientes
                                .Include(p => p.Ubicaciones)
                                 // .Include(p => p.DireccionPaciente) // Si usas DireccionPaciente, descomenta y asegura que también esté en Models
                                 // .ThenInclude(d => d.Comuna) // Si usas Comuna, descomenta y asegura que también esté en Models
                                 .FirstOrDefaultAsync(p => p.rut == cleanRut && p.dv.ToUpper() == cleanDv);
        }
    }
}