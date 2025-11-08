namespace Hospital.Api.Data.Entities
{
    public class PacienteHospital
    {
        public int Id { get; set; }
        public string Rut { get; set; } = string.Empty;
        public string Dv { get; set; } = string.Empty;
        public string PrimerNombre { get; set; } = string.Empty;
        public string SegundoNombre { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string ApellidoMaterno { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string Sexo { get; set; } = string.Empty;
        public string TelefonoMovil { get; set; } = string.Empty;
        public string? TelefonoFijo { get; set; }
        public string? Mail { get; set; }
        public bool? PRAIS { get; set; }

        // Relaciones mínimas
        public ICollection<SolicitudQuirurgica>? Solicitudes { get; set; }
    }
}
