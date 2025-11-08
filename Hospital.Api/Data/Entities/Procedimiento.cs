using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Hospital.Api.Data.Entities
{
    [Table("PROCEDIMIENTO")]
    public class Procedimiento
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("codigo")]
        public int Codigo { get; set; }

        [Column("nombre")]
        [StringLength(255)]
        public string Nombre { get; set; } = string.Empty;

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("TIPO_PROCEDIMIENTO_ID")]
        public int TipoProcedimientoId { get; set; }

        // 🔹 FK opcional: crea relación si existe la tabla TIPO_PROCEDIMIENTO
        [ForeignKey("TipoProcedimientoId")]
        public TipoProcedimiento? TipoProcedimiento { get; set; }
    }
}
