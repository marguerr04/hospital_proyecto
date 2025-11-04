using proyecto_hospital_version_1.Data;
using System; // Para DateTime

namespace proyecto_hospital_version_1.Data._Legacy
{
    public class Solicitud
    {
        public int Id { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public string Diagnostico { get; set; } = default!;
        public Procedencia Procedencia { get; set; }
        public bool EsGes { get; set; }
        public int PacienteId { get; set; }
        public Paciente? Paciente { get; set; }
    }
}