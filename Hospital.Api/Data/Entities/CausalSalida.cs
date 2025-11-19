// Data/Entities/CausalSalida.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.Api.Data.Entities
{
    [Table("CAUSAL_SALIDA")]
    public class CausalSalida
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("contactabilidad")]
        public bool? Contactabilidad { get; set; }
    }
}
