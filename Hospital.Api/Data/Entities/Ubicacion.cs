namespace Hospital.Api.Data.Entities
{
    public class Ubicacion
    {
        public int IdDomicilio { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string NomDireccion { get; set; } = string.Empty;
        public string NumDireccion { get; set; } = string.Empty;
        public bool? Ruralidad { get; set; }
        public int CiudadId { get; set; }
        public int PacienteId { get; set; }
        public int TipoViaId { get; set; }

        public Paciente Paciente { get; set; } = null!;
    }
}
