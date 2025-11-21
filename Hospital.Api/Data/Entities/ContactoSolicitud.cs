using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.Api.Data.Entities
{
    [Table("CONTACTO_SOLICITUD")]
    public class ContactoSolicitud
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("SOLICITUD_QUIRURGICA_CONSENTIMIENTO_INFORMADO_id")]
        public int SolicitudConsentimientoId { get; set; }

        [Column("SOLICITUD_QUIRURGICA_idSolicitud")]
        public int SOLICITUD_QUIRURGICA_idSolicitud { get; set; }

        [Required]
        [Column("fechaContacto")]
        public DateTime fechaContacto { get; set; }

        [Column("horaContacto")]
        public TimeSpan? horaContacto { get; set; }

        [Column("MEDIO_id")]
        public int? MedioId { get; set; }

        [Column("MOTIVO_CONTACTO_id")]
        public int? MotivoContactoId { get; set; }

        [Column("idProfesional")]
        public int? IdProfesional { get; set; }

        [Column("idPariente")]
        public int? IdPariente { get; set; }

        [Column("RELACION_PACIENTE_id")]
        public int? RelacionPacienteId { get; set; }

        [Column("descripcion")]
        [StringLength(500)]
        public string? descripcion { get; set; }

        // Navigation properties (opcional)
        [ForeignKey("MotivoContactoId")]
        public virtual MotivoContacto? MotivoContacto { get; set; }
    }
}
