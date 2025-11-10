// Hospital.Api/DTOs/SolicitudRecienteDto.cs
namespace Hospital.Api.DTOs // ¡Namespace corregido!
{
    public class SolicitudRecienteDto
    {
        public int SolicitudId { get; set; }
        public string PacienteNombreCompleto { get; set; }
        public string PacienteRut { get; set; }
        public string Prioridad { get; set; }
        public string PrioridadCssClass { get; set; }
        public bool EsGes { get; set; }
        public string DescripcionProcedimiento { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string TiempoTranscurrido { get; set; }
    }
}