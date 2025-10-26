using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyecto_hospital_version_1.Models
{
    [Table("DIAGNOSTICO")] 
    public class Diagnostico
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("codigo_cie")]
        public string CodigoCie { get; set; } = string.Empty;

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;
    }
}