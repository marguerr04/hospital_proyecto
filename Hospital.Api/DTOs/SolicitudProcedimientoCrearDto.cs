namespace Hospital.Api.DTOs
{
    public class SolicitudProcedimientoCrearDto
    {
        public int ProcedimientoId { get; set; }
        public bool EsPrincipal { get; set; } = false;
        public int? Orden { get; set; }
        public string? Observaciones { get; set; }
    }
}
