using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyecto_hospital_version_1.Models
{
    [Table("PATOLOGIA_GES")]
    public class PatologiaGes
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre")]
        [StringLength(255)]
        public string Nombre { get; set; } = string.Empty;

        // Propiedad de navegación para MapeoGes (opcional, pero útil)
        public ICollection<MapeoGes>? MapeosGes { get; set; }


    }
}