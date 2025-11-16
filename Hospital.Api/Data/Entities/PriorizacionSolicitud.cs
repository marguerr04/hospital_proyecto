using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.Api.Data.Entities
{
    [Table("PRIORIZACION_SOLICITUD")]
    public class PriorizacionSolicitud
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("fechaPriorizacion")]
        public DateTime FechaPriorizacion { get; set; }

        [Required]
        [Column("CRITERIO_PRIORIZACION_id")]
        public int CriterioPriorizacionId { get; set; }

        // ⚠️ Estos dos nombres DEBEN calzar con la BD:
        [Required]
        [Column("SOLICITUD_QUIRURGICA_idSolicitud")]
        public int SolicitudQuirurgicaId { get; set; }

        [Required]
        [Column("SOLICITUD_QUIRURGICA_CONSENTIMIENTO_INFORMADO_id")]
        public int SolicitudConsentimientoId { get; set; }

        [Column("MOTIVO_PRIORIZACION_id")]
        public int? MotivoPriorizacionId { get; set; }

        [Column("prioridad")]
        public int? Prioridad { get; set; }

        // Navegación
        [ForeignKey(nameof(CriterioPriorizacionId))]
        public virtual CriterioPriorizacion CriterioPriorizacion { get; set; } = null!;

        // OJO: relación compuesta contra la Alternate Key de SolicitudQuirurgicaReal
        public virtual SolicitudQuirurgicaReal SolicitudQuirurgica { get; set; } = null!;

        [ForeignKey(nameof(MotivoPriorizacionId))]
        public virtual MotivoPriorizacion? MotivoPriorizacion { get; set; }
    }
}
