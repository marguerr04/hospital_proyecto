using Hospital.Api.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.Api.Data.Entities
{
    [Table("SOLICITUD_QUIRURGICA")]
    public class SolicitudQuirurgicaReal
    {
        [Key]
        [Column("idSolicitud")]
        public int IdSolicitud { get; set; }

        [Column("idSIGTE")]
        public bool? IdSIGTE { get; set; }

        [Column("CONSENTIMIENTO_INFORMADO_id")]
        public int ConsentimientoId { get; set; }

        [Required]
        [Column("validacionGES")]
        public bool? ValidacionGES { get; set; }

        [Required]
        [Column("fechaCreacion")]
        public DateTime FechaCreacion { get; set; }

        [Column("DIAGNOSTICO_id")]
        public int DiagnosticoId { get; set; }

        [Required]
        [Column("validacionDuplicado")]
        public bool? ValidacionDuplicado { get; set; }

        [Column("PROCEDENCIA_id")]
        public int ProcedenciaId { get; set; }

        [Column("TIPO_PRESTACION_id")]
        public int TipoPrestacionId { get; set; }

        // Navigation properties
        [ForeignKey("ConsentimientoId")]
        public virtual ConsentimientoInformadoReal Consentimiento { get; set; }

        [ForeignKey("DiagnosticoId")]
        public virtual Diagnostico Diagnostico { get; set; }

        [ForeignKey("ProcedenciaId")]
        public virtual Procedencia Procedencia { get; set; }

        [ForeignKey("TipoPrestacionId")]
        public virtual TipoPrestacion TipoPrestacion { get; set; }

        public virtual DetalleClinicoReal DetalleClinico { get; set; }

        // ajuste
        public virtual ICollection<DetallePacienteReal> DetallesPaciente { get; set; } = new List<DetallePacienteReal>();

        // Trackeo Rol Solicitud

        public ICollection<SolicitudProfesional> Profesionales { get; set; }


    }
}