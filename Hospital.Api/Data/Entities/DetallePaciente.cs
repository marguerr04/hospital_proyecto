namespace Hospital.Api.Data.Entities
{
    public class DetallePaciente
    {
        public int Id { get; set; }
        public decimal Peso { get; set; }
        public decimal Altura { get; set; }
        public decimal IMC { get; set; }
        public int SolicitudConsentimientoId { get; set; }
        public int SolicitudId { get; set; }
    }
}
