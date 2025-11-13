using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.Api.Data.Entities
{
    [Table("MOTIVO_CONTACTO")]
    public class MotivoContacto
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("nombre")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        // Navigation property
        public virtual ICollection<Contactabilidad> Contactos { get; set; } = new List<Contactabilidad>();
    }
}