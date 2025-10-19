using System.ComponentModel.DataAnnotations.Schema;

namespace proyecto_hospital_version_1.Models
{
    [Table("PACIENTE")]
    public class PacienteHospital
    {
        public int id { get; set; }
        public string rut { get; set; }
        public string dv { get; set; }
        public string primerNombre { get; set; }
        public string segundoNombre { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public DateTime fechaNacimiento { get; set; }
        public string sexo { get; set; }
        public string telefonoMovil { get; set; }
        public string mail { get; set; }
        public bool? PRAIS { get; set; }
        public string? telefonoFijo { get; set; }
    }
}
