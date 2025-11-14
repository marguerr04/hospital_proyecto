// Data/Entities/Profesional.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.Api.Data.Entities
{
    [Table("PROFESIONAL")]
    public class Profesional
    {
        public int Id { get; set; }
        public string rut { get; set; }
        public string dv { get; set; }
        public string primerNombre { get; set; }
        public string segundoNombre { get; set; }
        public string primerApellido { get; set; }
        public string segundoApellido { get; set; }

        public ICollection<SolicitudProfesional> SolicitudesProfesionales { get; set; }
    }
}
