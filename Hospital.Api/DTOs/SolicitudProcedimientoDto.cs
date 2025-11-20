namespace Hospital.Api.DTOs
{
    public class SolicitudProcedimientoDto
    {
        public int SolicitudId { get; set; }
        public int ConsentimientoId { get; set; }
        public int ProcedimientoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Codigo { get; set; }
        public bool EsPrincipal { get; set; }
        public int Orden { get; set; }
        public string? Observaciones { get; set; }
    }


}
