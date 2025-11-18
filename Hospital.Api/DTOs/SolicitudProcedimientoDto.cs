namespace Hospital.Api.Data.DTOs
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

    public class SolicitudProcedimientoCrearDto
    {
        public int ProcedimientoId { get; set; }
        public bool EsPrincipal { get; set; } = false;
        public int? Orden { get; set; }
        public string? Observaciones { get; set; }
    }
}
