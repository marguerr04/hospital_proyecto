namespace Hospital.Api.DTOs
{
    public class ConsentimientoInformadoDto
    {
        public DateTime FechaGeneracion { get; set; }
        public bool Estado { get; set; }
        public int LateralidadId { get; set; }
        public int ExtremidadId { get; set; }
        public string? Observacion { get; set; }
        public int ProcedimientoId { get; set; }
        public int PacienteId { get; set; }
    }
}
