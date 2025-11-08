namespace Hospital.Api.Data.Entities
{

    public class PrevisionPaciente
    {
        public int Id { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaSalida { get; set; }
        public int PacienteId { get; set; }

        public Paciente Paciente { get; set; } = null!;
    }
}
