using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyecto_hospital_version_1.Models
{
    [Table("DIAGNOSTICO")] // Esto mapea a tu tabla exacta
    public class Diagnostico
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("Codificacion_id")]
        public string CodigoCie { get; set; } = string.Empty;

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;
    }
}