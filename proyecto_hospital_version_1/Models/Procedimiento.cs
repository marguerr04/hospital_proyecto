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

        [Column("codigo")] // antes definido como string, que era diferente de la bd
        public int Codigo { get; set; } 

        [Column("nombre")]
        [StringLength(255)]
        public string Nombre { get; set; } = string.Empty;

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("TIPO_PROCEDIMIENTO_ID")]
        public int TipoProcedimientoId { get; set; }

        [ForeignKey("TipoProcedimientoId")]
        public TipoProcedimiento? TipoProcedimiento { get; set; }

        // Si hay una tabla intermedia como PROCEDIMIENTO_SOLICITUD
        // public ICollection<ProcedimientoSolicitud> ProcedimientoSolicitudes { get; set; }
    }
}