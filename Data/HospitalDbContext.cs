using Microsoft.EntityFrameworkCore;
using proyecto_hospital_version_1.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyecto_hospital_version_1.Data.Hospital
{
    public class HospitalDbContext : DbContext
    {
        public HospitalDbContext(DbContextOptions<HospitalDbContext> options) : base(options) { }

        // mis dbsets (MANTENIDOS)
        public DbSet<PacienteHospital> Pacientes { get; set; }
        public DbSet<Ubicacion> Ubicaciones { get; set; }
        public DbSet<Diagnostico> Diagnosticos { get; set; } = default!;
        public DbSet<Procedimiento> Procedimientos { get; set; } = default!;
        public DbSet<TipoProcedimiento> TiposProcedimiento { get; set; } = default!;
        public DbSet<EspecialidadHospital> ESPECIALIDAD { get; set; } = default!;
        public DbSet<SolicitudQuirurgica> SolicitudesQuirurgicas { get; set; }

        // DbSet de mi compañero que si puedo ocupar (sin conflicto)
        public DbSet<Extremidad> EXTREMIDAD { get; set; }
        public DbSet<Lateralidad> LATERALIDAD { get; set; }
        public DbSet<CatalogoEstados> CATALOGO_ESTADOS { get; set; }
        public DbSet<CausalSalida> CAUSAL_SALIDA { get; set; }
        public DbSet<PrevisionTipo> TIPO_PREVISION { get; set; }

        // DbSets mi compañero que no debo usar (en conflicto con los mios)
        /*
        public DbSet<Paciente> PACIENTE { get; set; }
        public DbSet<SolicitudQuirurgica> SOLICITUD_QUIRURGICA { get; set; }
        public DbSet<ConsentimientoInformado> CONSENTIMIENTO_INFORMADO { get; set; }
        public DbSet<Prevision> PREVISION { get; set; }
        public DbSet<PrevisionPaciente> PREVISION_PACIENTE { get; set; }
        public DbSet<Ubicacion> UBICACION { get; set; }
        public DbSet<EstadoSolicitud> ESTADO_SOLICITUD { get; set; }
        public DbSet<DetallePaciente> DETALLE_PACIENTE { get; set; }
        public DbSet<DetalleClinico> DETALLE_CLINICO { get; set; }
        public DbSet<Solicitud> SOLICITUDES { get; set; } = null!;
        public DbSet<Diagnostico> DIAGNOSTICO { get; set; }
        */

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mis configuraciones (MANTENIDAS)
            modelBuilder.Entity<PacienteHospital>().ToTable("PACIENTE");
            modelBuilder.Entity<PacienteHospital>()
                .HasMany(p => p.Ubicaciones)
                .WithOne(u => u.Paciente)
                .HasForeignKey(u => u.PACIENTE_id);
            modelBuilder.Entity<Ubicacion>().ToTable("UBICACION");

            // Configuraciones de mi compañero que si puedo ocupar (sin conflicto)
            modelBuilder.Entity<Extremidad>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<Lateralidad>()
                .HasKey(l => l.Id);

            modelBuilder.Entity<CatalogoEstados>()
                .HasKey(ce => ce.Id);

            modelBuilder.Entity<CausalSalida>()
                .HasKey(cs => cs.Id);

            modelBuilder.Entity<PrevisionTipo>()
                .HasKey(pt => pt.Id);

            //  PRECISIONES PARA DETALLES (útil si luego agregas DetallePaciente)
            modelBuilder.Entity<DetallePaciente>(entity =>
            {
                entity.Property(e => e.Altura).HasPrecision(5, 2);
                entity.Property(e => e.Peso).HasPrecision(5, 2);
                entity.Property(e => e.IMC).HasPrecision(5, 2);
            });

            // ❌ CONFIGURACIONES DE TU COMPAÑERO QUE NO DEBES USAR (en conflicto)
            /*
            modelBuilder.Entity<DetallePaciente>()
                .HasKey(dp => new { dp.Id, dp.SolicitudConsentimientoId, dp.SolicitudId });

            modelBuilder.Entity<DetalleClinico>()
                .HasKey(dc => new { dc.SolicitudConsentimientoId, dc.SolicitudId });

            modelBuilder.Entity<EstadoSolicitud>()
                .HasKey(es => es.Id);

            modelBuilder.Entity<SolicitudQuirurgica>()
                .HasKey(s => s.IdSolicitud);

            modelBuilder.Entity<Solicitud>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<Ubicacion>()
                .HasKey(u => u.IdDomicilio);

            modelBuilder.Entity<ConsentimientoInformado>()
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

            modelBuilder.Entity<SolicitudQuirurgica>()
                .HasOne(s => s.Consentimiento)
                .WithMany(c => c.Solicitudes)
                .HasForeignKey(s => s.ConsentimientoId)
                .IsRequired();

            modelBuilder.Entity<Solicitud>()
                .HasOne(s => s.Paciente)
                .WithMany(p => p.Solicitudes)
                .HasForeignKey(s => s.PacienteId);

            modelBuilder.Entity<EstadoSolicitud>()
                .HasOne(es => es.CatalogoEstado)
                .WithMany()
                .HasForeignKey(es => es.CatalogoEstadosId);
            */
        }
    }

    // Clases de compñero que si puedo ocupar
    public class Extremidad
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }

    public class Lateralidad
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }

    public class CausalSalida
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool? Contactabilidad { get; set; }
    }

    public class PrevisionTipo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }

    public class CatalogoEstados
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }

    // CLASES DE SOPORTE (para las precisiones)
    public class DetallePaciente
    {
        public int Id { get; set; }
        public decimal Peso { get; set; }
        public decimal Altura { get; set; }
        public decimal IMC { get; set; }
        public int SolicitudConsentimientoId { get; set; }
        public int SolicitudId { get; set; }
    }

    //  Clases a itegrar, que estan en conflicto
    /*
    public class Paciente
    {
        public int Id { get; set; }
        public string Rut { get; set; } = string.Empty;
        public string Dv { get; set; } = string.Empty;
        public string PrimerNombre { get; set; } = string.Empty;
        public string SegundoNombre { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string ApellidoMaterno { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string Sexo { get; set; } = string.Empty;
        public string TelefonoMovil { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
        public bool? PRAIS { get; set; }
        public string TelefonoFijo { get; set; } = string.Empty;

        public ICollection<ConsentimientoInformado> Consentimientos { get; set; } = new List<ConsentimientoInformado>();
        public ICollection<PrevisionPaciente> Previsiones { get; set; } = new List<PrevisionPaciente>();
        public ICollection<Ubicacion> Ubicaciones { get; set; } = new List<Ubicacion>();
        public ICollection<Solicitud> Solicitudes { get; set; } = new List<Solicitud>();
    }

    public class SolicitudQuirurgica
    {
        public int IdSolicitud { get; set; }
        public bool? IdSIGTE { get; set; }
        public int ConsentimientoId { get; set; }
        public bool ValidacionGES { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int DiagnosticoId { get; set; }
        public bool ValidacionDuplicado { get; set; }
        public int ProcedenciaId { get; set; }
        public int ProcedenciaId2 { get; set; }
        public int TipoPrestacionId { get; set; }

        public ConsentimientoInformado Consentimiento { get; set; } = null!;
    }

    public class Solicitud
    {
        public int Id { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Diagnostico { get; set; } = string.Empty;
        public int Procedencia { get; set; }
        public bool EsGes { get; set; }
        public int PacienteId { get; set; }

        public Paciente Paciente { get; set; } = null!;
    }

    public class ConsentimientoInformado
    {
        public int Id { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public bool Estado { get; set; }
        public int LateralidadId { get; set; }
        public int ExtremidadId { get; set; }
        public string Observacion { get; set; } = string.Empty;
        public int ProcedimientoId { get; set; }
        public int PacienteId { get; set; }

        public Paciente Paciente { get; set; } = null!;
        public ICollection<SolicitudQuirurgica> Solicitudes { get; set; } = new List<SolicitudQuirurgica>();
    }

    public class Prevision
    {
        public int Id { get; set; }
        public string Rut { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public int TipoPrevisionId { get; set; }
    }

    public class PrevisionPaciente
    {
        public int Id { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaSalida { get; set; }
        public int PacienteId { get; set; }

        public Paciente Paciente { get; set; } = null!;
    }

    public class Ubicacion
    {
        public int IdDomicilio { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string NomDireccion { get; set; } = string.Empty;
        public string NumDireccion { get; set; } = string.Empty;
        public bool? Ruralidad { get; set; }
        public int CiudadId { get; set; }
        public int PacienteId { get; set; }
        public int TipoViaId { get; set; }

        public Paciente Paciente { get; set; } = null!;
    }

    public class DetalleClinico
    {
        public int SolicitudConsentimientoId { get; set; }
        public int SolicitudId { get; set; }
        public int TiempoEstimadoCirugia { get; set; }
        public bool? EvaluacionAnestesica { get; set; }
        public bool? EvaluacionTransfusion { get; set; }
    }

    public class EstadoSolicitud
    {
        public int Id { get; set; }
        public int SolicitudConsentimientoId { get; set; }
        public int SolicitudId { get; set; }
        public DateTime FechaComienzo { get; set; }
        public DateTime? FechaTermino { get; set; }
        public int CatalogoEstadosId { get; set; }

        public CatalogoEstados CatalogoEstado { get; set; } = null!;
    }
    */
}