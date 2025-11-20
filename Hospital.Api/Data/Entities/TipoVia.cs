using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.Api.Data.Entities
{
    [Table("TIPO_VIA")]
    public class TipoVia
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        // Relación inversa si quieres (opcional)
        public ICollection<Ubicacion> Ubicaciones { get; set; } = new List<Ubicacion>();
    }
}
