using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using proyecto_hospital_version_1.Models; 

namespace proyecto_hospital_version_1.Data

{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opts) : base(opts) { }

        public DbSet<Paciente> Pacientes => Set<Paciente>();
        public DbSet<Solicitud> Solicitudes => Set<Solicitud>();
    }

}
