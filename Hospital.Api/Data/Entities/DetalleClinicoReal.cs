using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyecto_hospital_version_1.Data.Entities
{
    [Table("DETALLE_CLINICO")]
    public class DetalleClinicoReal
    {
        // Clave primaria 1
        [Key, Column("SOLICITUD_QUIRURGICA_CONSENTIMIENTO_INFORMADO_id", Order = 0)]
        public int SolicitudConsentimientoId { get; set; }

        // Clave primaria 2 ( segun el modelo) 
        [Key, Column("SOLICITUD_QUIRURGICA_idSolicitud", Order = 1)]
        public int SolicitudId { get; set; }

        [Required]
        [Column("tiempoEstimadoCirugia")]
        public int TiempoEstimadoCirugia { get; set; }

        [Column("evaluacionAnestesica")]
        public bool? EvaluacionAnestesica { get; set; }

        [Column("evaluacionTransfusion")]
        public bool? EvaluacionTransfusion { get; set; }

        
        public virtual SolicitudQuirurgicaReal Solicitud { get; set; }
    }
}