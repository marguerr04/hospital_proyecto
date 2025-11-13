using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.Api.Data.Entities
{
    [Table("CONTACTABILIDAD")]
    public class Contactabilidad
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("SOLICITUD_QUIRURGICA_CONSENTIMIENTO_INFORMADO_id")]
        public int SolicitudConsentimientoId { get; set; }

        [Column("SOLICITUD_QUIRURGICA_idSolicitud")]
        public int SolicitudId { get; set; }

        [Required]
        [Column("fechaContacto")]
        public DateTime FechaContacto { get; set; }

        [Column("horaContacto")]
        public TimeSpan? HoraContacto { get; set; }

        [Column("MEDIO_id")]
        public int? MedioId { get; set; }

        [Required]
        [Column("MOTIVO_CONTACTO_id")]
        public int MotivoContactoId { get; set; }

        [Column("idProfesional")]
        public int? IdProfesional { get; set; }

        [Column("idPariente")]
        public int? IdPariente { get; set; }

        [Column("RELACION_PACIENTE_id")]
        public int? RelacionPacienteId { get; set; }

        [Column("descripcion")]
        [StringLength(500)]
        public string? Descripcion { get; set; }

        // Navigation properties
        [ForeignKey("MotivoContactoId")]
        public virtual MotivoContacto? MotivoContacto { get; set; }

        [ForeignKey("SolicitudId")]
        public virtual SolicitudQuirurgicaReal? Solicitud { get; set; }
    }
}