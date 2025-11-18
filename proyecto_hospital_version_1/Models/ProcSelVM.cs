namespace proyecto_hospital_version_1.Models
{
    // ViewModel para procedimientos secundarios seleccionados en la UI
    public class ProcSelVM
    {
        public int ProcedimientoId { get; set; }
        public int Codigo { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool EsPrincipal { get; set; } = false; // En esta interfaz todos son secundarios
        public int Orden { get; set; } = 1;
        public string? Observaciones { get; set; }
    }
}
