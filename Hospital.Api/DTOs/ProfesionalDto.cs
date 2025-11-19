namespace Hospital.Api.DTOs
{
    public class ProfesionalDto
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string RutCompleto { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty; 
        public string? Especialidad { get; set; } 
    }
}