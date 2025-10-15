using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
namespace proyecto_hospital_version_1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opts) : base(opts) { }

        public DbSet<Paciente> Pacientes => Set<Paciente>();
        public DbSet<Solicitud> Solicitudes => Set<Solicitud>();
    }

    public class Paciente
    {
        public int Id { get; set; }
        public string Rut { get; set; } = default!;
        public string Nombre { get; set; } = default!;
    }

    public enum Procedencia { Ambulatorio, Hospitalizado, Urgencia }

    public class Solicitud
    {
        public int Id { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public string Diagnostico { get; set; } = default!;
        public Procedencia Procedencia { get; set; }
        public bool EsGes { get; set; }    // regla simple: si Procedencia = Urgencia → true
        public int PacienteId { get; set; }
        public Paciente? Paciente { get; set; }
    }
}
