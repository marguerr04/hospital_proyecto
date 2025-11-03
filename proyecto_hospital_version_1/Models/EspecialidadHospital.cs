using System.ComponentModel.DataAnnotations.Schema;

namespace proyecto_hospital_version_1.Models
{
    [Table("ESPECIALIDAD")]
    public class EspecialidadHospital 
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}