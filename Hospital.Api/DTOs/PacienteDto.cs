namespace Hospital.Api.DTOs
{
    public class PacienteDto
    {
        public int Id { get; set; }
        public string Rut { get; set; } = "";
        public string Dv { get; set; } = "";
        public string RutCompleto { get; set; } = "";
        public string PrimerNombre { get; set; } = "";
        public string? SegundoNombre { get; set; }
        public string ApellidoPaterno { get; set; } = "";
        public string? ApellidoMaterno { get; set; }
        public string NombreCompleto { get; set; } = "";
        public DateTime FechaNacimiento { get; set; }
        public int Edad { get; set; }
        public string Sexo { get; set; } = "";
        public string TelefonoMovil { get; set; } = "";
        public string? TelefonoFijo { get; set; }
        public string Mail { get; set; } = "";
        public bool? PRAIS { get; set; }
        public List<UbicacionDto> Ubicaciones { get; set; } = new();

        // 👇 Nueva propiedad de conveniencia para las vistas
        public string EdadCompleta => $"{Edad} años";
    }
}
