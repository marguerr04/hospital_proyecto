namespace Hospital.Api.Data.DTOs
{
    public class PriorizacionDto
    {
        public int SolicitudId { get; set; }                // idSolicitud
        public int CriterioPriorizacionId { get; set; }     // ID del criterio seleccionado en el front
        public string Justificacion { get; set; } = string.Empty;
        public DateTime FechaPriorizacion { get; set; } = DateTime.Now;
        // (Opcional) Si quisieras forzar prioridad manual desde el front:
        // public int? Prioridad { get; set; }
    }
}