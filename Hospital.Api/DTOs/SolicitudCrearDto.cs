namespace Hospital.Api.DTOs
{
    public class SolicitudCrearDto
    {
        public int PacienteId { get; set; }
        public int? ConsentimientoId { get; set; } // 🔹 Se agregó

        // --- Datos base ---
        public string DiagnosticoPrincipal { get; set; } = string.Empty;
        public string ProcedimientoPrincipal { get; set; } = string.Empty;
        public string Procedencia { get; set; } = "Ambulatorio";

        // --- Datos clínicos ---
        public decimal Peso { get; set; }
        public decimal Talla { get; set; }
        public decimal IMC { get; set; }
        public int TiempoEstimado { get; set; }

        // --- Evaluaciones ---
        public bool EvaluacionAnestesica { get; set; }
        public bool EvaluacionTransfusion { get; set; }
        public bool EsGes { get; set; }

        // --- Contexto ---
        public string? Comentarios { get; set; }
        public string? EspecialidadOrigen { get; set; }
        public string? EspecialidadDestino { get; set; }

        // --- Compatibilidad ---
        public string? Lateralidad { get; set; }
        public string? Extremidad { get; set; }
    }
}
