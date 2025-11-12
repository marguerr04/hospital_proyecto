using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.Api.Data.Entities
{
    [Table("PROCEDENCIA")]
    public class Procedencia
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("nombre")]
        [StringLength(30)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [Column("cargaSigte")]
        public bool CargaSigte { get; set; }
    }
}