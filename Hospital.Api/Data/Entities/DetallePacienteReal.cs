using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyecto_hospital_version_1.Data.Entities
{
    [Table("DETALLE_PACIENTE_REAL")]
    public class DetallePacienteReal
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("peso")]
        public decimal Peso { get; set; }

        [Required]
        [Column("altura")]
        public decimal Altura { get; set; }

        [Required]
        [Column("IMC")]
        public decimal IMC { get; set; }

        [Column("SOLICITUD_QUIRURGICA_CONSENTIMIENTO_INFORMADO_id")]
        public int SolicitudConsentimientoId { get; set; }

        [Column("SOLICITUD_QUIRURGICA_idSolicitud")]
        public int SolicitudId { get; set; }

        
        // ForeignKey("SolicitudConsentimientoId, SolicitudId")] 
        public virtual SolicitudQuirurgicaReal Solicitud { get; set; }
    }
}