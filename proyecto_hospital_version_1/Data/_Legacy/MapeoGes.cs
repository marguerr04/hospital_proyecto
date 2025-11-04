using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System;
namespace proyecto_hospital_version_1.Data._Legacy
{
    [Table("MAPEO_GES")]
    public class MapeoGes
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("DIAGNOSTICO_ID")]
        public int DiagnosticoId { get; set; }

        [Column("PATOLOGIA_GES_ID")]
        public int PatologiaGesId { get; set; }

        [Column("fechaVigenciaInicial")]
        public DateTime? FechaVigenciaInicial { get; set; } // Nullable según tu BD

        [Column("fechaVigenciaFinal")]
        public DateTime? FechaVigenciaFinal { get; set; } // Nullable según tu BD

        // Propiedades de navegación
        [ForeignKey("DiagnosticoId")]
        public Diagnostico? Diagnostico { get; set; }

        [ForeignKey("PatologiaGesId")]
        public PatologiaGes? PatologiaGes { get; set; }
    }
}