namespace Hospital.Api.Data.Entities
{
    public class Prevision
    {
        public int Id { get; set; }
        public string Rut { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public int TipoPrevisionId { get; set; }
    }
}
