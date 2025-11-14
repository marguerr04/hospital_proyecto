using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.Api.Data.Entities
{
    [Table("PACIENTE")]
    public class Paciente
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("rut")]
        [StringLength(20)]
        public string Rut { get; set; } = string.Empty;

        [Required]
        [Column("dv")]
        [StringLength(1)]
        public string Dv { get; set; } = string.Empty;

        [Required]
        [Column("primerNombre")]
        [StringLength(100)]
        public string PrimerNombre { get; set; } = string.Empty;

        [Column("segundoNombre")]
        [StringLength(100)]
        public string? SegundoNombre { get; set; }

        [Required]
        [Column("apellidoPaterno")]
        [StringLength(100)]
        public string ApellidoPaterno { get; set; } = string.Empty;

        [Column("apellidoMaterno")]
        [StringLength(100)]
        public string? ApellidoMaterno { get; set; }

        [Required]
        [Column("fechaNacimiento")]
        public DateTime FechaNacimiento { get; set; }

        [Required]
        [Column("sexo")]
        [StringLength(1)]
        public string Sexo { get; set; } = string.Empty;

        [Required]
        [Column("telefonoMovil")]
        [StringLength(20)]
        public string TelefonoMovil { get; set; } = string.Empty;

        [Required]
        [Column("mail")]
        [StringLength(100)]
        public string Mail { get; set; } = string.Empty;

        [Column("PRAIS")]
        public bool? PRAIS { get; set; }

        [Column("telefonoFijo")]
        [StringLength(20)]
        public string? TelefonoFijo { get; set; }

        // ✅ Navigation Properties
        public virtual ICollection<ConsentimientoInformadoReal> Consentimientos { get; set; } = new List<ConsentimientoInformadoReal>();
        public virtual ICollection<PrevisionPaciente> Previsiones { get; set; } = new List<PrevisionPaciente>();
        public virtual ICollection<Ubicacion> Ubicaciones { get; set; } = new List<Ubicacion>();
        public virtual ICollection<Solicitud> Solicitudes { get; set; } = new List<Solicitud>();

        // ✅ Propiedades calculadas
        [NotMapped]
        public string NombreCompleto => $"{PrimerNombre} {SegundoNombre} {ApellidoPaterno} {ApellidoMaterno}".Replace("  ", " ").Trim();

        [NotMapped]
        public int Edad
        {
            get
            {
                var hoy = DateTime.Today;
                var edad = hoy.Year - FechaNacimiento.Year;
                if (FechaNacimiento.Date > hoy.AddYears(-edad)) edad--;
                return edad;
            }
        }

        [NotMapped]
        public string RutCompleto => $"{Rut}-{Dv}";
    }
}