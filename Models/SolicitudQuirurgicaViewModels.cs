using proyecto_hospital_version_1.Data.Hospital;

namespace proyecto_hospital_version_1.Models
{
    public class SolicitudQuirurgicaViewModel
    {
        public PacienteHospital? PacienteSeleccionado { get; set; }
        public string? Procedencia { get; set; } // Ej: "Ambulatorio", "Hospitalizado", "Urgencia"
        public string? DiagnosticoPrincipal { get; set; }
        public bool ConsentimientoAceptado { get; set; } = false;
        // Agrega aquí todas las demás propiedades de tu formulario
        // Ej: TipoDeCirugia, FechaEstimada, etc.
    }
}