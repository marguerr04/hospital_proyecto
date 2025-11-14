namespace Hospital.Api.Data.DTOs
{
    public class SolicitudDetalleDto
    {
        public int Id { get; set; }
        public string NombrePaciente { get; set; } = string.Empty;
        public string Rut { get; set; } = string.Empty;
        public string Diagnostico { get; set; } = string.Empty;
        public string Procedimiento { get; set; } = string.Empty;
        public string Procedencia { get; set; } = string.Empty;
        public bool EsGes { get; set; }
        public string Comorbilidades { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public int? Prioridad { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaPriorizacion { get; set; }

        // Campos adicionales del detalle
        public decimal? Peso { get; set; }
        public decimal? Talla { get; set; }
        public decimal? IMC { get; set; }
        public int? TiempoEstimado { get; set; }
        public bool EvaluacionAnestesica { get; set; }
        public bool EvaluacionTransfusion { get; set; }
        public string? Comentarios { get; set; }
    }
}