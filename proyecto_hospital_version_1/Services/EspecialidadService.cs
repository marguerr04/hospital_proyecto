using proyecto_hospital_version_1.Models;
namespace proyecto_hospital_version_1.Services
{
    public class EspecialidadService : IEspecialidadService
    {
        // Simula la lista fija. 
        // Más adelante puedes inyectar tu DbContext y traerlos de la BD.
        private readonly List<Especialidad> _especialidades = new List<Especialidad>
        {
            new Especialidad { Id = 1, Nombre = "Ortopedia" },
            new Especialidad { Id = 2, Nombre = "Otorrinolaringología" },
            new Especialidad { Id = 3, Nombre = "Urología" },
            new Especialidad { Id = 4, Nombre = "Cirugía Cardiovascular" },
            new Especialidad { Id = 5, Nombre = "Cirugía Plástica y Reconstructiva" },
            new Especialidad { Id = 6, Nombre = "Anestesiología" },
            new Especialidad { Id = 7, Nombre = "Medicina General" },
            new Especialidad { Id = 8, Nombre = "Pediatría" }
        };

        public async Task<IEnumerable<Especialidad>> GetEspecialidadesAsync()
        {
            // Simula una llamada asíncrona a la BD
            await Task.Delay(10); 
            return _especialidades;
        }
    }
}