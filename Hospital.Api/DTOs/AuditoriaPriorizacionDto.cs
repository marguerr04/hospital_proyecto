using System;

namespace Hospital.Api.DTOs
{
    /// <summary>
    /// DTO para el historial de auditoría de priorizaciones
    /// Tabla: AUD_PRIORIZACION_SOLICITUD
    /// </summary>
    public class AuditoriaPriorizacionDto
    {
        public int AudId { get; set; }
        public DateTime AudFecha { get; set; }
        public string AudAccion { get; set; } = string.Empty;
        public string AudUsuario { get; set; } = string.Empty;
        public int Id { get; set; }
        public DateTime FechaPriorizacion { get; set; }
        public int CriterioPriorizacionId { get; set; }
        public int SolicitudQuirurgicaConsentimientoInformadoId { get; set; }
        public int SolicitudQuirurgicaIdSolicitud { get; set; }
        public int MotivoPriorizacionId { get; set; }
        public byte Prioridad { get; set; }  // Cambiado de int a byte para coincidir con tinyint de BD
        public int? ResponsableProfesionalId { get; set; }
        public int? ResponsableRolSolicitudId { get; set; }
        public int? ResponsableRolHospitalId { get; set; }

        // Propiedades calculadas para la vista
        public string TiempoTranscurrido
        {
            get
            {
                var diff = DateTime.Now - AudFecha;
                if (diff.TotalDays >= 1)
                    return $"Hace {(int)diff.TotalDays} día(s)";
                if (diff.TotalHours >= 1)
                    return $"Hace {(int)diff.TotalHours} hora(s)";
                return $"Hace {(int)diff.TotalMinutes} minuto(s)";
            }
        }

        public string PrioridadTexto => $"P{Prioridad}";

        public string PrioridadCssClass => Prioridad switch
        {
            1 => "badge bg-danger",
            2 => "badge bg-warning text-dark",
            _ => "badge bg-primary"
        };
    }
}
