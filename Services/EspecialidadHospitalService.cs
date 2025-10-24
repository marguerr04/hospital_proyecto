using Microsoft.EntityFrameworkCore;
using proyecto_hospital_version_1.Data.Hospital;
using proyecto_hospital_version_1.Models; 

namespace proyecto_hospital_version_1.Services
{
    public class EspecialidadHospitalService : IEspecialidadHospital
    {
        private readonly HospitalDbContext _context;

        public EspecialidadHospitalService(HospitalDbContext context)
        {
            _context = context;
        }

        public async Task<List<EspecialidadHospital>> GetEspecialidadesAsync()
        {
            return await _context.ESPECIALIDAD.OrderBy(e => e.Nombre).ToListAsync();
        }



        // NUEVO MÉTODO - AGREGA ESTO
        public async Task<List<string>> GetDiagnosticosSugeridosAsync()
        {
            // Versión temporal con datos de ejemplo
            return await Task.FromResult(new List<string>
        {
            "Hipertensión arterial esencial",
            "Diabetes mellitus tipo 2",
            "Diabetes mellitus tipo 1",
            "Artrosis de rodilla",
            "Lumbalgia aguda",
            "Neumonía adquirida en la comunidad",
            "Infección urinaria no complicada",
            "Colelitiasis",
            "Apendicitis aguda",
            "Hernia inguinal",
            "Catarata senil"
        });
        }
    }
}