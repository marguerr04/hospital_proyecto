namespace Hospital.Api.Data.DTOs
{
    public class ProcedimientoDto
    {
        public int Id { get; set; }
        public int Codigo { get; set; }
        public string Nombre { get; set; } = "";
        public string Descripcion { get; set; } = "";
    }
}
