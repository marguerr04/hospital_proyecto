using Microsoft.EntityFrameworkCore;
using proyecto_hospital_version_1.Models; // <-- ¡NUEVO USING!
using System.ComponentModel.DataAnnotations.Schema;
namespace proyecto_hospital_version_1.Data.Hospital // Namespace de tu DbContext
{
    public class HospitalDbContext : DbContext
    {
        public HospitalDbContext(DbContextOptions<HospitalDbContext> options) : base(options) { }

        public DbSet<PacienteHospital> Pacientes { get; set; }
        public DbSet<Ubicacion> Ubicaciones { get; set; }

        public DbSet<Diagnostico> Diagnosticos { get; set; } = default!;
        public DbSet<Procedimiento> Procedimientos { get; set; } = default!;
        public DbSet<TipoProcedimiento> TiposProcedimiento { get; set; } = default!;
        public DbSet<EspecialidadHospital> ESPECIALIDAD { get; set; } = default!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<PacienteHospital>().ToTable("PACIENTE");

            modelBuilder.Entity<PacienteHospital>()
                .HasMany(p => p.Ubicaciones)         // Un PacienteHospital tiene muchas Ubicaciones
                .WithOne(u => u.Paciente)            // Una Ubicacion pertenece a un PacienteHospital
                .HasForeignKey(u => u.PACIENTE_id);  // La clave foránea en Ubicacion es PACIENTE_id

            modelBuilder.Entity<Ubicacion>().ToTable("UBICACION"); // Asegura mapeo de Ubicacion
        }
    }
}