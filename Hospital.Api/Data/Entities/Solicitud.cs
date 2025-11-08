namespace Hospital.Api.Data.Entities
{
    public class Solicitud
    {
        public int Id { get; set; }                       // PKs
        public DateTime FechaCreacion { get; set; }
        public string Diagnostico { get; set; } = string.Empty;
        public int Procedencia { get; set; }
        public bool EsGes { get; set; }
        public int PacienteId { get; set; }

        public Paciente Paciente { get; set; } = null!;
    }
}
