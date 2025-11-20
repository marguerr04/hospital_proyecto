using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.Api.Data.Entities
{
    [Table("UBICACION")] // Asegura que se mapea a la tabla UBICACION
    public class Ubicacion
    {
        [Key]
        [Column("idDomicilio")] // Nombre exacto de la columna PK
        public int IdDomicilio { get; set; }

        [Column("fechaRegistro")]
        public DateTime FechaRegistro { get; set; }

        [Column("nomDireccion")]
        public string NomDireccion { get; set; } = string.Empty;

        [Column("numDireccion")]
        public string NumDireccion { get; set; } = string.Empty;

        [Column("ruralidad")]
        public bool? Ruralidad { get; set; }

        // --- Claves Foráneas con nombres de columna de tu DB ---
        [Column("CIUDAD_id")]
        public int CiudadId { get; set; } // Propiedad en C#

        [Column("PACIENTE_id")]
        public int PacienteId { get; set; } // Propiedad en C#

        [Column("TIPO_VIA_id")]
        public int TipoViaId { get; set; } // Propiedad en C#

        // --- Propiedades de Navegación ---
        // Estas son las que EF Core usa para hacer los JOINs
        [ForeignKey(nameof(PacienteId))] // Vincula la propiedad PacienteId a esta navegación
        public virtual Paciente Paciente { get; set; } = null!;

        // Si tienes la entidad Ciudad, agrégala (asumo que sí por el CiudadId)
        // [ForeignKey(nameof(CiudadId))]
        // public virtual Ciudad Ciudad { get; set; } = null!;

        [ForeignKey(nameof(TipoViaId))] // Vincula la propiedad TipoViaId a esta navegación
        public virtual TipoVia TipoVia { get; set; } = null!;
    }
}