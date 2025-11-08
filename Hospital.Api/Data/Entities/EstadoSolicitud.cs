namespace Hospital.Api.Data.Entities
{
    public class EstadoSolicitud
    {
        public int Id { get; set; }
        public int SolicitudConsentimientoId { get; set; }
        public int SolicitudId { get; set; }
        public DateTime FechaComienzo { get; set; }
        public DateTime? FechaTermino { get; set; }
        public int CatalogoEstadosId { get; set; }

        public CatalogoEstados CatalogoEstado { get; set; } = null!;
    }
}
