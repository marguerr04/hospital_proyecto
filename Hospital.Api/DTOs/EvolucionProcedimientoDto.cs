namespace Hospital.Api.DTOs
{
    public class EvolucionProcedimientoDto
    {
        public string Fecha { get; set; } = string.Empty; // yyyy-MM-dd
        public int Cantidad { get; set; }
    }
}