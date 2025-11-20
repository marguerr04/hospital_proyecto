namespace Hospital.Api.DTOs
{
    public class DiagnosticoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string CodigoCie { get; set; } = string.Empty;
        public bool EsGes { get; set; } // calculado
    }
}