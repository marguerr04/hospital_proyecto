using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace proyecto_hospital_version_1.Data
{
    public class HospitalDbContext : DbContext
    {
        public HospitalDbContext(DbContextOptions<HospitalDbContext> options)
            : base(options) { }

        // Tablas principales
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

        // Tablas de catálogo
        public DbSet<Extremidad> EXTREMIDAD { get; set; }
        public DbSet<Lateralidad> LATERALIDAD { get; set; }
        public DbSet<CatalogoEstados> CATALOGO_ESTADOS { get; set; }
        public DbSet<CausalSalida> CAUSAL_SALIDA { get; set; }
        public DbSet<Diagnostico> DIAGNOSTICO { get; set; }
        public DbSet<PrevisionTipo> TIPO_PREVISION { get; set; }


        // en proceso de reemplazo de procedimientos
        public DbSet<Procedimiento> Procedimientos { get; set; } = null!;

        public DbSet<Procedimiento> PROCEDIMIENTO { get; set; } = null!;
        public DbSet<TipoProcedimiento> TIPO_PROCEDIMIENTO { get; set; } = null!;

        public DbSet<Especialidad> ESPECIALIDAD { get; set; } = null!;




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Claves primarias
            modelBuilder.Entity<DetallePaciente>()
                .HasKey(dp => new { dp.Id, dp.SolicitudConsentimientoId, dp.SolicitudId });

            modelBuilder.Entity<DetalleClinico>()
                .HasKey(dc => new { dc.SolicitudConsentimientoId, dc.SolicitudId });

            modelBuilder.Entity<EstadoSolicitud>()
                .HasKey(es => es.Id);

            modelBuilder.Entity<SolicitudQuirurgica>()
                .HasKey(s => s.Id); // aqui tenias definidio con Idsolicitud  mi modelo ocupa id, el funcional s => s.IdSolicitud

            modelBuilder.Entity<Solicitud>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<Ubicacion>()
                .HasKey(u => u.IdDomicilio);

            // Relaciones
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


            /* La tuve que comentar, ya que mi modelo de solicitudquirurgica no esta ascoado a un consentimiento informado
            modelBuilder.Entity<SolicitudQuirurgica>()
                .HasOne(s => s.Consentimiento)
                .WithMany(c => c.Solicitudes)
                .HasForeignKey(s => s.ConsentimientoId)
                .IsRequired();
            */

            modelBuilder.Entity<Solicitud>()
                .HasOne(s => s.Paciente)
                .WithMany(p => p.Solicitudes)
                .HasForeignKey(s => s.PacienteId);

            modelBuilder.Entity<EstadoSolicitud>()
                .HasOne(es => es.CatalogoEstado)
                .WithMany()
                .HasForeignKey(es => es.CatalogoEstadosId);

            modelBuilder.Entity<DetallePaciente>(entity =>
            {
                entity.Property(e => e.Altura).HasPrecision(5, 2);
                entity.Property(e => e.Peso).HasPrecision(5, 2);
                entity.Property(e => e.IMC).HasPrecision(5, 2);
            });


            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Procedimiento>().ToTable("PROCEDIMIENTO");
            modelBuilder.Entity<TipoProcedimiento>().ToTable("TIPO_PROCEDIMIENTO");


            modelBuilder.Entity<Especialidad>().ToTable("ESPECIALIDAD");




        }
    }

    // ===================== ENTIDADES =====================
    public class Paciente
    {
        public int Id { get; set; }  // es para los dashbiard, solicitudes, ocupar public si nueva conexion con base de datos, nombre de la tabla 
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


    /*
      
     El Modelo le faltan datos, mi modelo parece ser mas compatible  
     
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

    */



    public class Solicitud
    {
        public int Id { get; set; }                       // PK
        public DateTime FechaCreacion { get; set; }
        public string Diagnostico { get; set; } = string.Empty;
        public int Procedencia { get; set; }
        public bool EsGes { get; set; }
        public int PacienteId { get; set; }

        public Paciente Paciente { get; set; } = null!;
    }

    // Resto de las entidades con inicialización de strings y colecciones
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

    /*
     Reemplazo de procedimiento local para que este enlazada a base de datos , mi procedimiento
     

    public class Procedimiento
    {
        public int Id { get; set; }
        public int Codigo { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int TipoProcedimientoId { get; set; }
    }

     */


    [Table("PROCEDIMIENTO")]
    public class Procedimiento
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("codigo")]
        public int Codigo { get; set; }

        [Column("nombre")]
        [StringLength(255)]
        public string Nombre { get; set; } = string.Empty;

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("TIPO_PROCEDIMIENTO_ID")]
        public int TipoProcedimientoId { get; set; }

        // 🔹 FK opcional: crea relación si existe la tabla TIPO_PROCEDIMIENTO
        [ForeignKey("TipoProcedimientoId")]
        public TipoProcedimiento? TipoProcedimiento { get; set; }
    }

    [Table("TIPO_PROCEDIMIENTO")]
    public class TipoProcedimiento
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre")]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        public ICollection<Procedimiento> Procedimientos { get; set; } = new List<Procedimiento>();
    }







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

    public class Diagnostico
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }

    public class PrevisionTipo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
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

    public class DetallePaciente
    {
        public int Id { get; set; }
        public decimal Peso { get; set; }
        public decimal Altura { get; set; }
        public decimal IMC { get; set; }
        public int SolicitudConsentimientoId { get; set; }
        public int SolicitudId { get; set; }
    }

    public class DetalleClinico
    {
        public int SolicitudConsentimientoId { get; set; }
        public int SolicitudId { get; set; }
        public int TiempoEstimadoCirugia { get; set; }
        public bool? EvaluacionAnestesica { get; set; }
        public bool? EvaluacionTransfusion { get; set; }
    }

    public class CatalogoEstados
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
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
    // Las demás entidades (Prevision, Diagnostico, Lateralidad, Extremidad, Procedencia, PrevisionTipo, CausalSalida, Procedimiento) igual: inicializar strings con "".
    // Tablas y modelos de Martin


    // Solicitud quirurgica que es aplicable en mi caso Martin Guerrero 


    // Defino PacienteGHospital para que este de manera local en HospitalFBcontext.cs
    public class PacienteHospital
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
        public string? TelefonoFijo { get; set; }
        public string? Mail { get; set; }
        public bool? PRAIS { get; set; }

        // Relaciones mínimas
        public ICollection<SolicitudQuirurgica>? Solicitudes { get; set; }
    }





    public class SolicitudQuirurgica
    {
        public int Id { get; set; }
        public int PacienteId { get; set; }

        public PacienteHospital Paciente { get; set; }

        [Required]
        public string DiagnosticoPrincipal { get; set; } = string.Empty;

        public string? CodigoCie { get; set; }

        [Required]
        public string ProcedimientoPrincipal { get; set; } = string.Empty;

        [Required]
        public string Procedencia { get; set; } = "Ambulatorio";

        public bool EsGes { get; set; }
        public string? EspecialidadOrigen { get; set; }
        public string? EspecialidadDestino { get; set; }

        // CAMBIAR A NO NULLABLE con valores por defecto
        public decimal Peso { get; set; }
        public decimal Talla { get; set; }
        public decimal IMC { get; set; }

        public string? EquiposRequeridos { get; set; }
        public string? TipoMesa { get; set; }
        public bool EvaluacionAnestesica { get; set; }
        public bool Transfusiones { get; set; }

        [Required]
        public string SalaOperaciones { get; set; } = string.Empty;

        public int TiempoEstimado { get; set; }
        public string? Comorbilidades { get; set; }
        public string? ComentariosAdicionales { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public string Estado { get; set; } = "Pendiente";
        public int Prioridad { get; set; } = 0;
        public string? CreadoPor { get; set; }
        // saber la fecha de priorización
        public DateTime? FechaPriorizacion { get; set; }





    }

    // Migracion para la Pacinete hospital 
    [Table("ESPECIALIDAD")]
    public class Especialidad
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("nombre")]
        [StringLength(255)]
        public string Nombre { get; set; } = string.Empty;
    }


















}