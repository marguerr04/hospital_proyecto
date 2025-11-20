// Hospital.Api/DTOs/FechaProgramadaDto.cs
namespace Hospital.Api.DTOs // ¡Namespace corregido!
{
    public class FechaProgramadaDto
    {
        public int ProgramacionId { get; set; }
        public DateTime FechaProgramada { get; set; }
        public string FechaProgramadaFormateada { get; set; }
        public string PacienteNombreCompleto { get; set; }
        public string DescripcionProcedimiento { get; set; }
        public bool EsGes { get; set; }
        public TimeSpan HoraProgramada { get; set; }
        public string Pabellon { get; set; }
    }
}