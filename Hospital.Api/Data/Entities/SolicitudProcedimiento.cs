using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.Api.Data.Entities
{
    [Table("SOLICITUD_QUIRURGICA_PROCEDIMIENTO")]
    public class SolicitudProcedimiento
    {
        // PK compuesta (Solicitud + Consentimiento + Procedimiento)
        [Column("SOLICITUD_idSolicitud")]
        public int SolicitudId { get; set; }

        [Column("SOLICITUD_CONSENTIMIENTO_id")]
        public int SolicitudConsentimientoId { get; set; }

        [Column("PROCEDIMIENTO_id")]
        public int ProcedimientoId { get; set; }

        [Column("secuencia")]
        public int Orden { get; set; } = 1;

        [Column("esPrincipal")]
        public bool EsPrincipal { get; set; }

        [Column("fechaAsignacion")]
        public DateTime FechaAsignacion { get; set; } = DateTime.UtcNow;

        [Column("notas")]
        [StringLength(500)]
        public string? Observaciones { get; set; }

        // Columna no existe en BD, se ignora.
        [NotMapped]
        public int? TiempoEstimadoSegmento { get; set; }

        // Navegaciones
        public virtual Procedimiento? Procedimiento { get; set; }
    }
}
