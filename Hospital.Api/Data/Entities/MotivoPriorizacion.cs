using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.Api.Data.Entities
{
    [Table("MOTIVO_PRIORIZACION")]
    public class MotivoPriorizacion
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("nombre")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        // Navegación
        public virtual ICollection<PriorizacionSolicitud> Priorizaciones { get; set; } = new List<PriorizacionSolicitud>();
    }
}