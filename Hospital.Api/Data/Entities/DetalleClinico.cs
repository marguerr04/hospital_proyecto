namespace Hospital.Api.Data.Entities
{
    public class DetalleClinico
    {
        public int SolicitudConsentimientoId { get; set; }
        public int SolicitudId { get; set; }
        public int TiempoEstimadoCirugia { get; set; }
        public bool? EvaluacionAnestesica { get; set; }
        public bool? EvaluacionTransfusion { get; set; }
    }
}
