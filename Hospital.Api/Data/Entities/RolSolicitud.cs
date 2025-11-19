// Hospital.Api/Data/Entities/RolSolicitud.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic; // Si vas a tener listas de SolicitudProfesional

namespace Hospital.Api.Data.Entities
{
    [Table("ROL_SOLICITUD")] // Asegura que se mapea a la tabla ROL_SOLICITUD
    public class RolSolicitud
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("nombre")]
        [StringLength(100)]
        public string nombre { get; set; } = string.Empty;

        // Propiedad de navegación inversa (opcional, pero útil para EF Core)
        // Representa las solicitudes profesionales que usan esta especialidad/rol de solicitud
        public ICollection<SolicitudProfesional>? SolicitudesProfesionales { get; set; }
    }
}