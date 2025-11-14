// Hospital.Api/DTOs/SolicitudMedicoDto.cs
namespace Hospital.Api.DTOs
{
    public class SolicitudMedicoDto
    {
        public int Id { get; set; }
        public string NombrePaciente { get; set; } = string.Empty;
        public string Rut { get; set; } = string.Empty;
        public string Diagnostico { get; set; } = string.Empty;
        public string Procedimiento { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaProgramada { get; set; }
        public int? DiasRestantes { get; set; }
        public string? Contactabilidad { get; set; }
    }
}