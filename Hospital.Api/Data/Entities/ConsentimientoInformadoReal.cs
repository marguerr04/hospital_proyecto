using Hospital.Api.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyecto_hospital_version_1.Data.Entities
{
    [Table("CONSENTIMIENTO_INFORMADO_REAL")]
    public class ConsentimientoInformadoReal
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("fechaGeneracion")]
        public DateTime FechaGeneracion { get; set; }

        [Required]
        [Column("estado")]
        public bool Estado { get; set; }

        [Column("LATERALIDAD_id")]
        public int LateralidadId { get; set; }

        [Column("EXTREMIDAD_id")]
        public int ExtremidadId { get; set; }

        [Column("observacion")]
        [StringLength(500)]
        public string? Observacion { get; set; }

        [Column("PROCEDIMIENTO_id")]
        public int ProcedimientoId { get; set; }

        [Column("PACIENTE_id")]
        public int PacienteId { get; set; }

        // Navigation properties
        [ForeignKey("PacienteId")]
        public virtual Paciente Paciente { get; set; }

        [ForeignKey("ProcedimientoId")]
        public virtual Procedimiento Procedimiento { get; set; }

        [ForeignKey("LateralidadId")]
        public virtual Lateralidad Lateralidad { get; set; }

        [ForeignKey("ExtremidadId")]
        public virtual Extremidad Extremidad { get; set; }

        public virtual ICollection<SolicitudQuirurgicaReal> Solicitudes { get; set; } = new List<SolicitudQuirurgicaReal>();
    }
}