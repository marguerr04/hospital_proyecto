using Hospital.Api.Data;
using Hospital.Api.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital.Api.Data.Services
{
    public interface IAuditoriaPriorizacionService
    {
        Task<List<AuditoriaPriorizacionDto>> GetHistorialAuditoriaAsync(int pageNumber = 1, int pageSize = 20);
        Task<int> GetTotalRegistrosAsync();
    }

    public class AuditoriaPriorizacionService : IAuditoriaPriorizacionService
    {
        private readonly HospitalDbContext _context;

        public AuditoriaPriorizacionService(HospitalDbContext context)
        {
            _context = context;
        }

        public async Task<List<AuditoriaPriorizacionDto>> GetHistorialAuditoriaAsync(int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var query = @"
                    SELECT 
                        [AudId],
                        [AudFecha],
                        [AudAccion],
                        [AudUsuario],
                        [id],
                        [fechaPriorizacion],
                        [CRITERIO_PRIORIZACION_id],
                        [SOLICITUD_QUIRURGICA_CONSENTIMIENTO_INFORMADO_id],
                        [SOLICITUD_QUIRURGICA_idSolicitud],
                        [MOTIVO_PRIORIZACION_id],
                        [prioridad],
                        [ResponsableProfesionalId],
                        [ResponsableRolSolicitudId],
                        [ResponsableRolHospitalId]
                    FROM [dbo].[AUD_PRIORIZACION_SOLICITUD]
                    ORDER BY [AudFecha] DESC
                    OFFSET {0} ROWS
                    FETCH NEXT {1} ROWS ONLY";

                var offset = (pageNumber - 1) * pageSize;

                var result = await _context.Database
                    .SqlQueryRaw<AuditoriaPriorizacionQueryResult>(query, offset, pageSize)
                    .ToListAsync();

                return result.Select(r => new AuditoriaPriorizacionDto
                {
                    AudId = r.AudId,
                    AudFecha = r.AudFecha,
                    AudAccion = r.AudAccion,
                    AudUsuario = r.AudUsuario,
                    Id = r.Id,
                    FechaPriorizacion = r.FechaPriorizacion,
                    CriterioPriorizacionId = r.CRITERIO_PRIORIZACION_id,
                    SolicitudQuirurgicaConsentimientoInformadoId = r.SOLICITUD_QUIRURGICA_CONSENTIMIENTO_INFORMADO_id,
                    SolicitudQuirurgicaIdSolicitud = r.SOLICITUD_QUIRURGICA_idSolicitud,
                    MotivoPriorizacionId = r.MOTIVO_PRIORIZACION_id,
                    Prioridad = r.Prioridad,
                    ResponsableProfesionalId = r.ResponsableProfesionalId,
                    ResponsableRolSolicitudId = r.ResponsableRolSolicitudId,
                    ResponsableRolHospitalId = r.ResponsableRolHospitalId
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetHistorialAuditoriaAsync: {ex.Message}");
                return new List<AuditoriaPriorizacionDto>();
            }
        }

        public async Task<int> GetTotalRegistrosAsync()
        {
            try
            {
                // Enfoque alternativo: Usar ADO.NET directamente para mayor control
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT COUNT(*) FROM [dbo].[AUD_PRIORIZACION_SOLICITUD]";
                    
                    await _context.Database.OpenConnectionAsync();
                    
                    var result = await command.ExecuteScalarAsync();
                    
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetTotalRegistrosAsync: {ex.Message}");
                return 0;
            }
            finally
            {
                await _context.Database.CloseConnectionAsync();
            }
        }
    }

    // Clase auxiliar para mapear los resultados de la query
    public class AuditoriaPriorizacionQueryResult
    {
        public int AudId { get; set; }
        public DateTime AudFecha { get; set; }
        public string AudAccion { get; set; } = string.Empty;
        public string AudUsuario { get; set; } = string.Empty;
        public int Id { get; set; }
        public DateTime FechaPriorizacion { get; set; }
        public int CRITERIO_PRIORIZACION_id { get; set; }
        public int SOLICITUD_QUIRURGICA_CONSENTIMIENTO_INFORMADO_id { get; set; }
        public int SOLICITUD_QUIRURGICA_idSolicitud { get; set; }
        public int MOTIVO_PRIORIZACION_id { get; set; }
        public byte Prioridad { get; set; }  // Cambiado de int a byte para coincidir con tinyint de BD
        public int? ResponsableProfesionalId { get; set; }
        public int? ResponsableRolSolicitudId { get; set; }
        public int? ResponsableRolHospitalId { get; set; }
    }
}
