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

        [Required]
        [Column("SOLICITUD_QUIRURGICA_idSolicitud")]
        public int SolicitudQuirurgicaId { get; set; }

        [Column("MOTIVO_PRIORIZACION_id")]
        public int? MotivoPriorizacionId { get; set; }

        [Required]
        [Column("prioridad")]
        public int Prioridad { get; set; }

        // Navegación
        [ForeignKey("CriterioPriorizacionId")]
        public virtual CriterioPriorizacion CriterioPriorizacion { get; set; } = null!;

        [ForeignKey("SolicitudQuirurgicaId")]
        public virtual SolicitudQuirurgicaReal SolicitudQuirurgica { get; set; } = null!;

        [ForeignKey("MotivoPriorizacionId")]
        public virtual MotivoPriorizacion? MotivoPriorizacion { get; set; }
    }
}