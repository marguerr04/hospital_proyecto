using System;
using System.ComponentModel.DataAnnotations;

namespace proyecto_hospital_version_1.Models
{
    public class SolicitudQuirurgica
    {
        public int Id { get; set; }

        // Relación con Paciente
        public int PacienteId { get; set; }
        public Paciente Paciente { get; set; }

        // Datos del formulario
        public string DiagnosticoPrincipal { get; set; }
        public string CodigoCie { get; set; }
        public string ProcedimientoPrincipal { get; set; }
        public string Procedencia { get; set; }
        public bool EsGes { get; set; }

        // Especialidades
        public string EspecialidadOrigen { get; set; }
        public string EspecialidadDestino { get; set; }

        // Datos clínicos
        public decimal? Peso { get; set; }
        public decimal? Talla { get; set; }
        public decimal? IMC { get; set; }

        // Equipos y recursos (como JSON o string delimitado)
        public string EquiposRequeridos { get; set; } // "Arco C,Torre Lap,Sutura"
        public string TipoMesa { get; set; }

        // Evaluaciones
        public bool EvaluacionAnestesica { get; set; }
        public bool Transfusiones { get; set; }

        // Planificación
        public string SalaOperaciones { get; set; }
        public int? TiempoEstimado { get; set; }

        // Comorbilidades y comentarios
        public string Comorbilidades { get; set; } // "Diabetes,HTA,Obesidad"
        public string ComentariosAdicionales { get; set; }

        // Metadata
        public DateTime FechaCreacion { get; set; }
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Priorizada, Aprobada
        public int Prioridad { get; set; } = 0; // 0=Normal, 1=Alta, 2=Urgente
        public string CreadoPor { get; set; } // Usuario que creó
    }
}