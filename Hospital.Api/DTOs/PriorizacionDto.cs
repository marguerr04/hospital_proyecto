namespace Hospital.Api.Data.DTOs
{
    public class PriorizacionDto
    {
        public int SolicitudId { get; set; }
        public int Prioridad { get; set; }
        public string CriterioPriorizacion { get; set; } = string.Empty; // "ges", "prais", etc.
        public string Justificacion { get; set; } = string.Empty;
        public DateTime FechaPriorizacion { get; set; } = DateTime.Now;
    }
}