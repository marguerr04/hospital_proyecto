using Microsoft.EntityFrameworkCore;
using proyecto_hospital_version_1.Data._Legacy;

// se cambiara el db context referenciado de mi proyecto a legacy para evitar confuciones y conflicots con la api


namespace proyecto_hospital_version_1.Services
{
    public class EspecialidadHospitalService : IEspecialidadHospital
    {
        private readonly HospitalDbContextLegacy _context;

        public EspecialidadHospitalService(HospitalDbContextLegacy context)
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