namespace Hospital.Api.Data.Entities
{
    public class Paciente
    {
        public int Id { get; set; }  // es para los dashboard, solicitudes, ocupar public si nueva conexion con base de datos, nombre de la tabla 
        public string Rut { get; set; } = string.Empty;
        public string Dv { get; set; } = string.Empty;
        public string PrimerNombre { get; set; } = string.Empty;
        public string SegundoNombre { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string ApellidoMaterno { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string Sexo { get; set; } = string.Empty;
        public string TelefonoMovil { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
        public bool? PRAIS { get; set; }
        public string TelefonoFijo { get; set; } = string.Empty;

        public ICollection<ConsentimientoInformadoReal> Consentimientos { get; set; } = new List<ConsentimientoInformadoReal>();
        public ICollection<PrevisionPaciente> Previsiones { get; set; } = new List<PrevisionPaciente>();
        public ICollection<Ubicacion> Ubicaciones { get; set; } = new List<Ubicacion>();
        public ICollection<Solicitud> Solicitudes { get; set; } = new List<Solicitud>();

    }
}
