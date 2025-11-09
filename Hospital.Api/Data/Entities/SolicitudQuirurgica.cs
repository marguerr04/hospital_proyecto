using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

// referencia a solicitudes quirurgicas
namespace Hospital.Api.Data.Entities
{
    [Table("SolicitudesQuirurgicas")]
    public class SolicitudQuirurgica
    {
        public int Id { get; set; }
        public int PacienteId { get; set; }

        public PacienteHospital Paciente { get; set; }

        [Required]
        public string DiagnosticoPrincipal { get; set; } = string.Empty;

        public string? CodigoCie { get; set; }

        [Required]
        public string ProcedimientoPrincipal { get; set; } = string.Empty;

        [Required]
        public string Procedencia { get; set; } = "Ambulatorio";

        public bool EsGes { get; set; }
        public string? EspecialidadOrigen { get; set; }
        public string? EspecialidadDestino { get; set; }

        // CAMBIAR A NO NULLABLE con valores por defecto
        public decimal Peso { get; set; }
        public decimal Talla { get; set; }
        public decimal IMC { get; set; }

        public string? EquiposRequeridos { get; set; }
        public string? TipoMesa { get; set; }
        public bool EvaluacionAnestesica { get; set; }
        public bool Transfusiones { get; set; }

        [Required]
        public string SalaOperaciones { get; set; } = string.Empty;

        public int TiempoEstimado { get; set; }
        public string? Comorbilidades { get; set; }
        public string? ComentariosAdicionales { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public string Estado { get; set; } = "Pendiente";
        public int Prioridad { get; set; } = 0;
        public string? CreadoPor { get; set; }
        // saber la fecha de priorización
        public DateTime? FechaPriorizacion { get; set; }





    }
}
