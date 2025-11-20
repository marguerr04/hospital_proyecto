namespace Hospital.Api.DTOs
{
    public class UbicacionDto
    {
        public int IdDomicilio { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string NomDireccion { get; set; } = "";
        public string NumDireccion { get; set; } = "";
        public bool? Ruralidad { get; set; }
        public int CiudadId { get; set; }
        public int TipoViaId { get; set; }

        public string DireccionCompleta => $"{NomDireccion} {NumDireccion}";
    }
}
