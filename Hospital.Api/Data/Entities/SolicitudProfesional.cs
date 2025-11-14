// Data/Entities/SolicitudProfesional.cs
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.Api.Data.Entities
{
    [Table("SOLICITUD_PROFESIONAL")]
    public class SolicitudProfesional
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }

        public int ROL_HOSPITAL_id { get; set; }
        public int PROFESIONAL_id { get; set; }
        public int ROL_SOLICITUD_id { get; set; }

        public int SOLICITUD_QUIRURGICA_CONSENTIMIENTO_INFORMADO_id { get; set; }
        public int SOLICITUD_QUIRURGICA_idSolicitud { get; set; }

        // Navegación
        public Profesional Profesional { get; set; }
        public SolicitudQuirurgicaReal Solicitud { get; set; }
    }
}
