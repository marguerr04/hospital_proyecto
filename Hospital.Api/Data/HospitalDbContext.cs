using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Hospital.Api.Data.Entities;
using proyecto_hospital_version_1.Data.Entities;


namespace Hospital.Api.Data
{
    public class HospitalDbContext : DbContext
    {
        public HospitalDbContext(DbContextOptions<HospitalDbContext> options)
            : base(options) { }

        // Tablas principales
        public DbSet<Paciente> PACIENTE { get; set; }
        // temporal public DbSet<SolicitudQuirurgica> SOLICITUD_QUIRURGICA { get; set; }
        // temporal public DbSet<ConsentimientoInformado> CONSENTIMIENTO_INFORMADO { get; set; }
        public DbSet<Prevision> PREVISION { get; set; }
        public DbSet<PrevisionPaciente> PREVISION_PACIENTE { get; set; }
        public DbSet<Ubicacion> UBICACION { get; set; }
        public DbSet<EstadoSolicitud> ESTADO_SOLICITUD { get; set; }
        // temporal public DbSet<DetallePaciente> DETALLE_PACIENTE { get; set; }
        // temporal public DbSet<DetalleClinico> DETALLE_CLINICO { get; set; }

        public DbSet<Solicitud> SOLICITUDES { get; set; } = null!;

        // Tablas de catálogo
        public DbSet<Extremidad> EXTREMIDAD { get; set; }
        public DbSet<Lateralidad> LATERALIDAD { get; set; }
        public DbSet<CatalogoEstados> CATALOGO_ESTADOS { get; set; }
        public DbSet<CausalSalida> CAUSAL_SALIDA { get; set; }
        public DbSet<Diagnostico> DIAGNOSTICO { get; set; }
        public DbSet<PrevisionTipo> TIPO_PREVISION { get; set; }


        // en proceso de reemplazo de procedimientos
        /* da conflicto, y en la bd no existe procedimientos como tal
        public DbSet<Procedimiento> Procedimientos { get; set; } = null!;
        */
        public DbSet<Procedimiento> PROCEDIMIENTO { get; set; } = null!;
        public DbSet<TipoProcedimiento> TIPO_PROCEDIMIENTO { get; set; } = null!;

        public DbSet<Especialidad> ESPECIALIDAD { get; set; } = null!;

        // Para la integracion


        public DbSet<SolicitudQuirurgicaReal> SOLICITUD_QUIRURGICA { get; set; } // ahora reemplazara  el original contolador
        public DbSet<ConsentimientoInformadoReal> CONSENTIMIENTO_INFORMADO { get; set; } // ahora reemplazara  el original contolador
        public DbSet<DetalleClinicoReal> DETALLE_CLINICO { get; set; } // ahora reemplazara  el original contolador
        public DbSet<DetallePacienteReal> DETALLE_PACIENTE { get; set; } // ahora reemplazara  el original contolador
        public DbSet<Procedencia> PROCEDENCIA { get; set; }
        public DbSet<TipoPrestacion> TIPO_PRESTACION { get; set; }

       // public DbSet<PriorizacionSolicitud> PRIORIZACION_SOLICITUD { get; set; }

        //public DbSet<CriterioPriorizacion> CRITERIO_PRIORIZACION { get; set; }

        //public DbSet<ProgramacionQuirurgica> PROGRAMACION_QUIRURGICA { get; set; }

        //public DbSet<Cirugia> CIRUGIA { get; set; }

        //public DbSet<EstadoProgramacion> ESTADO_PROGRAMACION { get; set; }


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

            // detalle clinico en la base de datos no tiene llave primaria, sjuj lalve primaria es relacion 1 a 1 con solicitud
            // ademas de que es llave primaria compuesta
            modelBuilder.Entity<DetalleClinicoReal>()
                .HasKey(d => new { d.SolicitudConsentimientoId, d.SolicitudId });

            modelBuilder.Entity<DetalleClinicoReal>()
                .HasOne(d => d.Solicitud)
                .WithOne(s => s.DetalleClinico)
                .HasForeignKey<DetalleClinicoReal>(d => d.SolicitudId)  
                .HasPrincipalKey<SolicitudQuirurgicaReal>(s => s.IdSolicitud);




            modelBuilder.Entity<Procedimiento>().ToTable("PROCEDIMIENTO");
            modelBuilder.Entity<TipoProcedimiento>().ToTable("TIPO_PROCEDIMIENTO");


            modelBuilder.Entity<Especialidad>().ToTable("ESPECIALIDAD");


            // cambios para la llave
            modelBuilder.Entity<DetallePacienteReal>()
                .HasKey(dp => new { dp.Id, dp.SolicitudConsentimientoId, dp.SolicitudId });


            modelBuilder.Entity<DetallePacienteReal>()
                .HasOne(dp => dp.Solicitud)
                .WithMany(sq => sq.DetallesPaciente)
                .HasForeignKey(dp => dp.SolicitudId)  
                .HasPrincipalKey(sq => sq.IdSolicitud);


            // para 2 consentimientos informados
            



            base.OnModelCreating(modelBuilder);
        }
    }

    // ===================== ENTIDADES =====================
    // Paciente
   

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

    // Se refactorizo Extremidad
    // Solicitud ( solo solicitud , no solicitudquirurgica)
    

    // Resto de las entidades con inicialización de strings y colecciones
    // ConsentimientoInformado
    // Prevision
   

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

    // Procedimiento
    

    //TipoProcedimiento







   
    // Se refactorizo Lateralidad
    
    // Causal salida
    
    // Diagnostico
   

    // PrevisionTipo
    //PrevisionPaciente

    // Ubicacion
    // DetallePaciente
    
    // DetalleClinico
    
    // Catalogo Estados
    
    // Estado Solicitud
   
    // Las demás entidades (Prevision, Diagnostico, Lateralidad, Extremidad, Procedencia, PrevisionTipo, CausalSalida, Procedimiento) igual: inicializar strings con "".
    // Tablas y modelos de Martin


    // Solicitud quirurgica que es aplicable en mi caso Martin Guerrero 


    // Defino PacienteGHospital para que este de manera local en HospitalFBcontext.cs
    // PacienteHospital
    




    // Solicitudes quirugicas sew llama SolicitudQuirurgica
    // Tabla Especialidad
    // Migracion para la Pacinete hospital 
    




}