namespace Hospital.Api.Data.Entities
{
    public class CausalSalida
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool? Contactabilidad { get; set; }
    }
}
