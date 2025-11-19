// Data/Entities/EgresoSolicitud.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.Api.Data.Entities
{
    [Table("EGRESO_SOLICITUD")]
    public class EgresoSolicitud
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("fechaSalida")]
        public DateTime FechaSalida { get; set; }

        [Column("respaldo")]
        public string? Respaldo { get; set; }

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("CAUSAL_SALIDA_id")]
        public int CausalSalidaId { get; set; }

        [Column("SOLICITUD_QUIRURGICA_CONSENTIMIENTO_INFORMADO_id")]
        public int SolicitudConsentimientoId { get; set; }

        [Column("SOLICITUD_QUIRURGICA_idSolicitud")]
        public int SolicitudId { get; set; }

        [Column("idProfesional")]
        public int IdProfesional { get; set; }

        // Nav
        public virtual CausalSalida? CausalSalida { get; set; }
    }
}
