// Hospital.Api/Data/Entities/RolHospital.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic; // Si vas a tener listas de SolicitudProfesional

namespace Hospital.Api.Data.Entities
{
    [Table("ROL_HOSPITAL")] // Asegura que se mapea a la tabla ROL_HOSPITAL
    public class RolHospital
    {
        [Key] // Marca 'Id' como la clave primaria
        [Column("id")] // Mapea a la columna 'id' en la base de datos
        public int Id { get; set; }

        [Required] // Hace que el campo 'nombre' sea obligatorio
        [Column("nombre")] // Mapea a la columna 'nombre'
        [StringLength(100)] // Opcional: define la longitud máxima del string
        public string nombre { get; set; } = string.Empty; // Propiedad en minúscula como en tu esquema

        [Column("descripcion")] // Mapea a la columna 'descripcion'
        [StringLength(500)] // Opcional
        public string? descripcion { get; set; } // Puede ser nulo

        // Propiedad de navegación inversa (opcional, pero útil para EF Core)
        // Representa las solicitudes profesionales que usan este rol
        public ICollection<SolicitudProfesional>? SolicitudesProfesionales { get; set; }
    }
}