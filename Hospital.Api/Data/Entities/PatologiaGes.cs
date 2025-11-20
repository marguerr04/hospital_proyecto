namespace Hospital.Api.Data.Entities
{
    public class PatologiaGes
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        // 
        public virtual ICollection<MapeoGes> MapeosGes { get; set; } = new List<MapeoGes>();
    }
}