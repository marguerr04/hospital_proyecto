using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Hospital.Api.Data.Entities
{
    [Table("ESPECIALIDAD")]
    public class Especialidad
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("nombre")]
        [StringLength(255)]
        public string Nombre { get; set; } = string.Empty;
    }
}
