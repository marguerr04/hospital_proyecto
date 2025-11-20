namespace Hospital.Api.Data.Entities
{
    public class MapeoGes
    {
        public int Id { get; set; }
        public int DiagnosticoId { get; set; }
        public int PatologiaGesId { get; set; }
        public DateTime FechaVigenciaInicial { get; set; }
        public DateTime? FechaVigenciaFinal { get; set; }

        // Navigation properties
        public virtual Diagnostico Diagnostico { get; set; } = null!;
        public virtual PatologiaGes PatologiaGes { get; set; } = null!;
    }
}