namespace Hospital.Api.Data.Entities
{
    public class ConsentimientoInformado
    {
        public int Id { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public bool Estado { get; set; }
        public int LateralidadId { get; set; }
        public int ExtremidadId { get; set; }
        public string Observacion { get; set; } = string.Empty;
        public int ProcedimientoId { get; set; }
        public int PacienteId { get; set; }

        public Paciente Paciente { get; set; } = null!;
        public ICollection<SolicitudQuirurgica> Solicitudes { get; set; } = new List<SolicitudQuirurgica>();
    }
}
