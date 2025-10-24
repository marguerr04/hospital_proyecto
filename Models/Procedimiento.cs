using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyecto_hospital_version_1.Models
{
    [Table("PROCEDIMIENTO")]
    public class Procedimiento
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("codigo")]
        [StringLength(50)]
        public string Codigo { get; set; } = string.Empty;

        [Column("nombre")]
        [StringLength(255)]
        public string Nombre { get; set; } = string.Empty;

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("tipoProcedimiento_id")]
        public int TipoProcedimientoId { get; set; }

        [ForeignKey("TipoProcedimientoId")]
        public TipoProcedimiento? TipoProcedimiento { get; set; }

        // Si hay una tabla intermedia como PROCEDIMIENTO_SOLICITUD
        // public ICollection<ProcedimientoSolicitud> ProcedimientoSolicitudes { get; set; }
    }
}