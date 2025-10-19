using Microsoft.EntityFrameworkCore;
using proyecto_hospital_version_1.Models; // <-- ¡NUEVO USING!
using System.ComponentModel.DataAnnotations.Schema;
namespace proyecto_hospital_version_1.Data.Hospital // Namespace de tu DbContext
{
    public class HospitalDbContext : DbContext
    {
        public HospitalDbContext(DbContextOptions<HospitalDbContext> options) : base(options) { }

        public DbSet<PacienteHospital> Pacientes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<PacienteHospital>().ToTable("PACIENTE");
        }
    }
}