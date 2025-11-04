using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyecto_hospital_version_1.Data._Legacy
{
    [Table("PACIENTE")]
    public class PacienteHospital
    {
        public int id { get; set; }
        public string rut { get; set; }
        public string dv { get; set; }
        public string primerNombre { get; set; }
        public string segundoNombre { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public DateTime fechaNacimiento { get; set; }
        public string sexo { get; set; }
        public string telefonoMovil { get; set; }
        public string mail { get; set; }
        public bool? PRAIS { get; set; }
        public string? telefonoFijo { get; set; }






        public ICollection<Ubicacion>? Ubicaciones { get; set; }

        // Propiedad calculada para el Nombre Completo
        [NotMapped]
        public string NombreCompleto => $"{primerNombre} {segundoNombre} {apellidoPaterno} {apellidoMaterno}".Replace("  ", " ").Trim();

        // Propiedad calculada para la Edad
        [NotMapped]
        public string EdadCompleta
        {
            get
            {
                var hoy = DateTime.Today;
                var edad = hoy.Year - fechaNacimiento.Year;
                var meses = hoy.Month - fechaNacimiento.Month;
                var dias = hoy.Day - fechaNacimiento.Day;

                if (dias < 0)
                {
                    meses--;
                    dias += DateTime.DaysInMonth(hoy.Year, hoy.Month - 1);
                }
                if (meses < 0)
                {
                    edad--;
                    meses += 12;
                }

                string edadStr = "";
                if (edad > 0) edadStr += $"{edad} año{(edad != 1 ? "s" : "")}";
                if (meses > 0) edadStr += $" {meses} mes{(meses != 1 ? "es" : "")}";
                if (dias > 0) edadStr += $" {dias} día{(dias != 1 ? "s" : "")}";

                return edadStr.Trim();
            }
        }
    }

    [Table("UBICACION")]
    public class Ubicacion
    {
        [Key]
        public int idDomicilio { get; set; }
        public DateTime fechaRegistro { get; set; }
        public string nomDireccion { get; set; } = default!;
        public string? numDireccion { get; set; }
        public bool? ruralidad { get; set; }

        public int CIUDAD_id { get; set; }
        // public Ciudad Ciudad { get; set; } // Si tienes un modelo Ciudad

        public int PACIENTE_id { get; set; }
        [ForeignKey("PACIENTE_id")]
        public PacienteHospital? Paciente { get; set; } // Referencia de navegación inversa

        public int TIPO_VIA_id { get; set; }
        // public TipoVia TipoVia { get; set; } // Si tienes un modelo TipoVia

        // Propiedad calculada para la dirección completa
        [NotMapped]
        public string DireccionCompleta => $"{nomDireccion} {(string.IsNullOrEmpty(numDireccion) ? "" : numDireccion)}";
    }
}
