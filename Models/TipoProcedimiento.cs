using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyecto_hospital_version_1.Models
{
    [Table("TIPO_PROCEDIMIENTO")]
    public class TipoProcedimiento
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre")]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        public ICollection<Procedimiento> Procedimientos { get; set; } = new List<Procedimiento>();
    }
}