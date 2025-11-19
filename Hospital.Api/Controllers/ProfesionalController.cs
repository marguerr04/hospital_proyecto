// E:\Retorno_cero\Testeo_recuperacion_1\Hospital.Api\Controllers\ProfesionalController.cs

// Asegúrate de que estos usings sean correctos para tu proyecto
using Hospital.Api.DTOs; // Para el ProfesionalDto
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hospital.Api.Data; // <-- Este es tu DbContext, corregido el using
// También necesitas los usings para tus entidades si están en un namespace diferente a Data
// Si tus entidades (Profesional, SolicitudProfesional, RolHospital, RolSolicitud)
// están en Hospital.Api.Data.Entities, entonces el using correcto es:
using Hospital.Api.Data.Entities; // <-- ¡Este es muy importante ahora!

using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System; // Para DateTime

namespace Hospital.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfesionalController : ControllerBase
    {
        // El tipo de DbContext ahora es HospitalDbContext, como en tu archivo
        private readonly HospitalDbContext _context;

        public ProfesionalController(HospitalDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene una lista de profesionales con su rol de hospital y especialidad de solicitud más reciente.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProfesionalDto>>> GetProfesionales()
        {
            var profesionalesData = await _context.PROFESIONAL // ¡Aquí usas .PROFESIONAL (mayúsculas) como está en tu DbContext!
                .GroupJoin(
                    _context.SOLICITUD_PROFESIONAL, // ¡Aquí usas .SOLICITUD_PROFESIONAL!
                    p => p.Id,
                    sp => sp.PROFESIONAL_id, // Propiedad de navegación en SolicitudProfesional
                    (p, spGroup) => new { Profesional = p, Solicitudes = spGroup }
                )
                .SelectMany(
                    p_sp => p_sp.Solicitudes.DefaultIfEmpty(), // LEFT JOIN con SolicitudProfesional
                    (p_sp, sp) => new
                    {
                        p_sp.Profesional.Id,
                        p_sp.Profesional.primerNombre, // Corregido a primerNombre (minúsculas)
                        p_sp.Profesional.segundoNombre, // Corregido a segundoNombre
                        p_sp.Profesional.primerApellido, // Corregido a primerApellido
                        p_sp.Profesional.segundoApellido, // Corregido a segundoApellido
                        p_sp.Profesional.rut, // Corregido a rut
                        p_sp.Profesional.dv, // Corregido a dv
                        SolicitudProfesionalId = sp != null ? sp.Id : (int?)null,
                        SpFecha = sp != null ? sp.Fecha : (DateTime?)null,
                        RolHospitalId = sp != null ? sp.ROL_HOSPITAL_id : (int?)null, // Corregido a ROL_HOSPITAL_id
                        RolSolicitudId = sp != null ? sp.ROL_SOLICITUD_id : (int?)null // Corregido a ROL_SOLICITUD_id
                    }
                )
                .GroupJoin(
                    _context.ROL_HOSPITAL, // ¡Aquí usas .ROL_HOSPITAL!
                    temp => temp.RolHospitalId,
                    rh => rh.Id,
                    (temp, rhGroup) => new { temp, RolesHospital = rhGroup }
                )
                .SelectMany(
                    temp_rh => temp_rh.RolesHospital.DefaultIfEmpty(), // LEFT JOIN con RolHospital
                    (temp_rh, rh) => new
                    {
                        temp_rh.temp.Id,
                        temp_rh.temp.primerNombre,
                        temp_rh.temp.segundoNombre,
                        temp_rh.temp.primerApellido,
                        temp_rh.temp.segundoApellido,
                        temp_rh.temp.rut,
                        temp_rh.temp.dv,
                        temp_rh.temp.SpFecha,
                        temp_rh.temp.RolSolicitudId,
                        RolNombre = rh != null ? rh.nombre : "Sin Rol Asignado" // Corregido a .nombre (minúsculas)
                    }
                )
                .GroupJoin(
                    _context.ROL_SOLICITUD, // ¡Aquí usas .ROL_SOLICITUD! (Asumiendo que has agregado este DbSet)
                    temp2 => temp2.RolSolicitudId,
                    rs => rs.Id,
                    (temp2, rsGroup) => new { temp2, RolesSolicitud = rsGroup }
                )
                .SelectMany(
                    temp2_rs => temp2_rs.RolesSolicitud.DefaultIfEmpty(), // LEFT JOIN con RolSolicitud
                    (temp2_rs, rs) => new
                    {
                        temp2_rs.temp2.Id,
                        temp2_rs.temp2.primerNombre,
                        temp2_rs.temp2.segundoNombre,
                        temp2_rs.temp2.primerApellido,
                        temp2_rs.temp2.segundoApellido,
                        temp2_rs.temp2.rut,
                        temp2_rs.temp2.dv,
                        temp2_rs.temp2.SpFecha,
                        RolNombre = temp2_rs.temp2.RolNombre,
                        EspecialidadNombre = rs != null ? rs.nombre : "Sin Especialidad Asignada" // Corregido a .nombre (minúsculas)
                    }
                )
                .ToListAsync();

            // Filtrar en memoria para obtener la solicitud más reciente por profesional
            var result = profesionalesData
                .GroupBy(p => p.Id)
                .Select(g => g.OrderByDescending(x => x.SpFecha ?? DateTime.MinValue).First())
                .Select(p => new ProfesionalDto
                {
                    Id = p.Id,
                    NombreCompleto = $"{p.primerNombre} {p.segundoNombre ?? ""} {p.primerApellido} {p.segundoApellido ?? ""}".Trim(),
                    RutCompleto = $"{p.rut}-{p.dv}",
                    Rol = p.RolNombre,
                    Especialidad = p.EspecialidadNombre
                })
                .OrderBy(p => p.NombreCompleto)
                .ToList();

            return Ok(result);
        }
    }
}