using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Hospital.Api.Data.Entities;

namespace Hospital.Api.Data
{
    public class HospitalDbContext : DbContext
    {
        public HospitalDbContext(DbContextOptions<HospitalDbContext> options)
            : base(options) { }

        // Tablas principales
        public DbSet<Paciente> PACIENTE { get; set; }
        public DbSet<Prevision> PREVISION { get; set; }
        public DbSet<PrevisionPaciente> PREVISION_PACIENTE { get; set; }
        public DbSet<Ubicacion> UBICACION { get; set; }
        public DbSet<EstadoSolicitud> ESTADO_SOLICITUD { get; set; }
        public DbSet<Solicitud> SOLICITUDES { get; set; } = null!;

        // Catálogos
        public DbSet<Extremidad> EXTREMIDAD { get; set; }
        public DbSet<Lateralidad> LATERALIDAD { get; set; }
        public DbSet<CatalogoEstados> CATALOGO_ESTADOS { get; set; }
        public DbSet<CausalSalida> CAUSAL_SALIDA { get; set; }
        public DbSet<Diagnostico> DIAGNOSTICO { get; set; }
        public DbSet<PrevisionTipo> TIPO_PREVISION { get; set; }

        // Contactabilidad
        public DbSet<Contactabilidad> CONTACTABILIDAD { get; set; } = null!;
        public DbSet<MotivoContacto> MOTIVO_CONTACTO { get; set; } = null!;

        // Procedimientos
        public DbSet<Procedimiento> PROCEDIMIENTO { get; set; } = null!;
        public DbSet<TipoProcedimiento> TIPO_PROCEDIMIENTO { get; set; } = null!;
        public DbSet<Especialidad> ESPECIALIDAD { get; set; } = null!;
        public DbSet<SolicitudProcedimiento> SOLICITUD_QUIRURGICA_PROCEDIMIENTO { get; set; } = null!;

        // Integración 
        public DbSet<SolicitudQuirurgicaReal> SOLICITUD_QUIRURGICA { get; set; }
        public DbSet<ConsentimientoInformadoReal> CONSENTIMIENTO_INFORMADO { get; set; }
        public DbSet<DetalleClinicoReal> DETALLE_CLINICO { get; set; }
        public DbSet<DetallePacienteReal> DETALLE_PACIENTE { get; set; }
        public DbSet<Procedencia> PROCEDENCIA { get; set; }
        public DbSet<TipoPrestacion> TIPO_PRESTACION { get; set; }

        // Priorización
        public DbSet<CriterioPriorizacion> CRITERIO_PRIORIZACION { get; set; } = null!;
        public DbSet<MotivoPriorizacion> MOTIVO_PRIORIZACION { get; set; } = null!;
        public DbSet<PriorizacionSolicitud> PRIORIZACION_SOLICITUD { get; set; } = null!;

        // Egreso solicitud 
        public DbSet<EgresoSolicitud> EGRESO_SOLICITUD { get; set; } = null!;


        // Rol Solicitud
        public DbSet<Profesional> PROFESIONAL { get; set; }
        public DbSet<SolicitudProfesional> SOLICITUD_PROFESIONAL { get; set; }


        // falta

        public DbSet<RolHospital> ROL_HOSPITAL { get; set; }
        public DbSet<RolSolicitud> ROL_SOLICITUD { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // === Claves existentes (dejadas como estaban) ===
            modelBuilder.Entity(typeof(DetallePaciente)).HasKey("Id", "SolicitudConsentimientoId", "SolicitudId");
            modelBuilder.Entity(typeof(DetalleClinico)).HasKey("SolicitudConsentimientoId", "SolicitudId");
            modelBuilder.Entity<EstadoSolicitud>().HasKey(es => es.Id);
            modelBuilder.Entity<SolicitudQuirurgica>().HasKey("Id");
            modelBuilder.Entity<Solicitud>().HasKey(s => s.Id);
            modelBuilder.Entity<Ubicacion>().HasKey(u => u.IdDomicilio);

            // === Relaciones existentes (respetadas) ===
            modelBuilder.Entity<ConsentimientoInformadoReal>()
                .HasOne(c => c.Paciente)
                .WithMany(p => p.Consentimientos)
                .HasForeignKey(c => c.PacienteId);

            modelBuilder.Entity<PrevisionPaciente>()
                .HasOne(pp => pp.Paciente)
                .WithMany(p => p.Previsiones)
                .HasForeignKey(pp => pp.PacienteId);

            modelBuilder.Entity<Ubicacion>()
                .HasOne(u => u.Paciente)
                .WithMany(p => p.Ubicaciones)
                .HasForeignKey(u => u.PacienteId);

            // Precisión de campos (dejado igual)
            modelBuilder.Entity<DetallePaciente>(entity =>
            {
                entity.Property(e => e.Altura).HasPrecision(5, 2);
                entity.Property(e => e.Peso).HasPrecision(5, 2);
                entity.Property(e => e.IMC).HasPrecision(5, 2);
            });

            modelBuilder.Entity<Procedimiento>().ToTable("PROCEDIMIENTO");
            modelBuilder.Entity<TipoProcedimiento>().ToTable("TIPO_PROCEDIMIENTO");
            modelBuilder.Entity<Especialidad>().ToTable("ESPECIALIDAD");



            //  PK compuesta de SOLICITUD_QUIRURGICA
            modelBuilder.Entity<SolicitudQuirurgicaReal>()
                .HasKey(s => s.IdSolicitud);

            modelBuilder.Entity<SolicitudQuirurgicaReal>()
                .HasAlternateKey(s => new { s.IdSolicitud, s.ConsentimientoId })
                .HasName("AK_SOLICITUD_QUIRURGICA_Id_Consent");


            modelBuilder.Entity<DetalleClinicoReal>()
                .HasKey(d => new { d.SolicitudConsentimientoId, d.SolicitudId });

            modelBuilder.Entity<DetalleClinicoReal>()
                .HasOne(d => d.Solicitud)
                .WithOne(s => s.DetalleClinico)
                .HasForeignKey<DetalleClinicoReal>(d => new { d.SolicitudId, d.SolicitudConsentimientoId })
                .HasPrincipalKey<SolicitudQuirurgicaReal>(s => new { s.IdSolicitud, s.ConsentimientoId });

            //  DETALLE_PACIENTE_REAL con FK compuesta a SOLICITUD_QUIRURGICA
            modelBuilder.Entity<DetallePacienteReal>()
                .HasKey(dp => new { dp.Id, dp.SolicitudConsentimientoId, dp.SolicitudId });

            modelBuilder.Entity<DetallePacienteReal>()
                .HasOne(dp => dp.Solicitud)
                .WithMany(sq => sq.DetallesPaciente)
                .HasForeignKey(dp => new { dp.SolicitudId, dp.SolicitudConsentimientoId })
                .HasPrincipalKey(sq => new { sq.IdSolicitud, sq.ConsentimientoId });

            //  PRIORIZACION_SOLICITUD con FK compuesta a SOLICITUD_QUIRURGICA
            modelBuilder.Entity<PriorizacionSolicitud>()
                .HasOne(p => p.SolicitudQuirurgica)
                .WithMany(s => s.Priorizaciones)
                .HasForeignKey(p => new { p.SolicitudQuirurgicaId, p.SolicitudConsentimientoId })
                .HasPrincipalKey(s => new { s.IdSolicitud, s.ConsentimientoId });


            modelBuilder.Entity<SolicitudProfesional>()
                .HasOne(sp => sp.Solicitud)
                .WithMany(s => s.Profesionales)
                .HasForeignKey(sp => sp.SOLICITUD_QUIRURGICA_idSolicitud);

            modelBuilder.Entity<SolicitudProfesional>()
                .HasOne(sp => sp.Profesional)
                .WithMany(p => p.SolicitudesProfesionales)
                .HasForeignKey(sp => sp.PROFESIONAL_id);


            // egreso solicitud

            modelBuilder.Entity<EgresoSolicitud>()
                .HasOne(e => e.CausalSalida)
                .WithMany()
                .HasForeignKey(e => e.CausalSalidaId);



            // (FK compuesta)
            modelBuilder.Entity<SolicitudProcedimiento>(entity =>
            {
                entity.HasKey(e => new { e.SolicitudId, e.SolicitudConsentimientoId, e.ProcedimientoId });
                entity.HasOne<SolicitudQuirurgicaReal>()
                      .WithMany(s => s.Procedimientos)
                      .HasForeignKey(e => new { e.SolicitudId, e.SolicitudConsentimientoId })
                      .HasPrincipalKey(s => new { s.IdSolicitud, s.ConsentimientoId });
                entity.HasOne(e => e.Procedimiento)
                      .WithMany()
                      .HasForeignKey(e => e.ProcedimientoId);
            });

            // egreso soliciutd

            






            base.OnModelCreating(modelBuilder);
        }
    }
}
