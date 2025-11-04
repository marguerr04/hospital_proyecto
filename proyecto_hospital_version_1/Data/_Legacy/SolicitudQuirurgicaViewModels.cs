namespace proyecto_hospital_version_1.Data._Legacy
{
    public class SolicitudQuirurgicaViewModel
    {
        public PacienteHospital? PacienteSeleccionado { get; set; }
        public string? Procedencia { get; set; } // Ej: "Ambulatorio", "Hospitalizado", "Urgencia"
        public string? DiagnosticoPrincipal { get; set; }
        public bool ConsentimientoAceptado { get; set; } = false;

    }
}