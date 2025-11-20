using Hospital.Api.Data;
using Hospital.Api.DTOs;
using Hospital.Api.Data.Services;
using Microsoft.EntityFrameworkCore;
using Hospital.Api.Data.Entities;

namespace Hospital.Api.Data.Services
{
    public class SolicitudQuirurgicaRealService : ISolicitudQuirurgicaService
    {
        private readonly HospitalDbContext _context;

        public SolicitudQuirurgicaRealService(HospitalDbContext context)
        {
            _context = context;
        }

        // 🔹 MÉTODO PRINCIPAL - MANTENIDO IGUAL
        public async Task<bool> CrearSolicitudAsync(
            int pacienteId,
            int? consentimientoId,
            string diagnosticoPrincipal,
            string procedimientoPrincipal,
            string procedencia,
            decimal peso,
            decimal talla,
            decimal imc,
            int tiempoEstimado,
            bool evaluacionAnestesica,
            bool evaluacionTransfusion,
            bool esGes,
            string? comentarios,
            string? especialidadOrigen,
            string? especialidadDestino,
            string? lateralidad,
            string? extremidad)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                Console.WriteLine($"🔍 [Service] Creando solicitud - ConsentimientoId: {consentimientoId}");

                // 1️⃣ VALIDAR CONSENTIMIENTO ESPECÍFICO
                if (consentimientoId == null || consentimientoId == 0)
                {
                    Console.WriteLine("❌ No se proporcionó un ConsentimientoId válido");
                    return false;
                }

                var consentimiento = await _context.CONSENTIMIENTO_INFORMADO
                    .FirstOrDefaultAsync(c => c.Id == consentimientoId && c.PacienteId == pacienteId);

                if (consentimiento == null)
                {
                    Console.WriteLine($"❌ Consentimiento {consentimientoId} no existe para paciente {pacienteId}");
                    return false;
                }

                Console.WriteLine($"✅ Consentimiento validado - ID: {consentimiento.Id}");

                // 2️⃣ OBTENER IDs DE TABLAS RELACIONADAS
                var diagnosticoId = await _context.DIAGNOSTICO
                    .Where(d => d.Nombre == diagnosticoPrincipal)
                    .Select(d => d.Id)
                    .FirstOrDefaultAsync();

                var procedenciaId = await _context.PROCEDENCIA
                    .Where(p => p.Nombre == procedencia)
                    .Select(p => p.Id)
                    .FirstOrDefaultAsync();

                var tipoPrestacionKey = "Cirugía Urgencia"; // Valor seguro por defecto

                if (!string.IsNullOrWhiteSpace(especialidadDestino))
                {
                    // Solo cambiar si coincide con un tipo de prestación real
                    var tipoPrestacionValida = await _context.TIPO_PRESTACION
                        .Where(t => t.Nombre.ToLower() == especialidadDestino.ToLower())
                        .Select(t => t.Nombre)
                        .FirstOrDefaultAsync();

                    if (tipoPrestacionValida != null)
                        tipoPrestacionKey = tipoPrestacionValida;
                }

                // Buscar el ID final de tipo de prestación
                var tipoPrestacionId = await _context.TIPO_PRESTACION
                    .Where(t => t.Nombre == tipoPrestacionKey)
                    .Select(t => t.Id)
                    .FirstOrDefaultAsync();

                // 3️⃣ VALIDAR IDs OBTENIDOS
                if (diagnosticoId == 0)
                {
                    Console.WriteLine($"❌ Diagnóstico '{diagnosticoPrincipal}' no encontrado");
                    return false;
                }

                if (procedenciaId == 0)
                {
                    Console.WriteLine($"❌ Procedencia '{procedencia}' no encontrada");
                    return false;
                }

                if (tipoPrestacionId == 0)
                {
                    Console.WriteLine($"❌ Tipo prestación '{especialidadDestino}' no encontrado");
                    return false;
                }

                Console.WriteLine($"✅ IDs obtenidos - Diagnostico: {diagnosticoId}, Procedencia: {procedenciaId}, TipoPrestacion: {tipoPrestacionId}");

                // 4️⃣ CREAR SOLICITUD QUIRÚRGICA REAL
                var solicitud = new SolicitudQuirurgicaReal
                {
                    ConsentimientoId = consentimientoId.Value,
                    DiagnosticoId = diagnosticoId,
                    ProcedenciaId = procedenciaId,
                    TipoPrestacionId = tipoPrestacionId,
                    FechaCreacion = DateTime.Now,
                    ValidacionGES = esGes,
                    ValidacionDuplicado = false,
                    IdSIGTE = true
                };

                _context.SOLICITUD_QUIRURGICA.Add(solicitud);
                await _context.SaveChangesAsync();

                Console.WriteLine($"✅ SolicitudReal creada - ID: {solicitud.IdSolicitud}");

                // 5️⃣ CREAR DETALLE PACIENTE
                var detallePaciente = new DetallePacienteReal
                {
                    SolicitudConsentimientoId = consentimientoId.Value,
                    SolicitudId = solicitud.IdSolicitud,
                    Peso = peso,
                    Altura = talla,
                    IMC = imc
                };

                _context.DETALLE_PACIENTE.Add(detallePaciente);

                // 6️⃣ CREAR DETALLE CLÍNICO
                var detalleClinico = new DetalleClinicoReal
                {
                    SolicitudConsentimientoId = consentimientoId.Value,
                    SolicitudId = solicitud.IdSolicitud,
                    TiempoEstimadoCirugia = tiempoEstimado,
                    EvaluacionAnestesica = evaluacionAnestesica,
                    EvaluacionTransfusion = evaluacionTransfusion
                };

                _context.DETALLE_CLINICO.Add(detalleClinico);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                Console.WriteLine($"🎉 SOLICITUD COMPLETA CREADA - ID: {solicitud.IdSolicitud}");
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"💥 ERROR: {ex.Message}");
                Console.WriteLine($"💥 StackTrace: {ex.StackTrace}");
                return false;
            }
        }

        // ✅ NUEVO: igual que el anterior, pero devuelve el ID de la solicitud creada (o 0 en error)
        public async Task<int> CrearSolicitudYDevolverIdAsync(
            int pacienteId,
            int? consentimientoId,
            string diagnosticoPrincipal,
            string procedimientoPrincipal,
            string procedencia,
            decimal peso,
            decimal talla,
            decimal imc,
            int tiempoEstimado,
            bool evaluacionAnestesica,
            bool evaluacionTransfusion,
            bool esGes,
            string? comentarios,
            string? especialidadOrigen,
            string? especialidadDestino,
            string? lateralidad,
            string? extremidad)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Reutilizamos exactamente la misma lógica que el método anterior, pero capturando el ID generado
                if (consentimientoId == null || consentimientoId == 0)
                    return 0;

                var consentimiento = await _context.CONSENTIMIENTO_INFORMADO
                    .FirstOrDefaultAsync(c => c.Id == consentimientoId && c.PacienteId == pacienteId);
                if (consentimiento == null)
                    return 0;

                var diagnosticoId = await _context.DIAGNOSTICO
                    .Where(d => d.Nombre == diagnosticoPrincipal)
                    .Select(d => d.Id)
                    .FirstOrDefaultAsync();
                var procedenciaId = await _context.PROCEDENCIA
                    .Where(p => p.Nombre == procedencia)
                    .Select(p => p.Id)
                    .FirstOrDefaultAsync();

                var tipoPrestacionKey = "Cirugía Urgencia";
                if (!string.IsNullOrWhiteSpace(especialidadDestino))
                {
                    var tipoPrestacionValida = await _context.TIPO_PRESTACION
                        .Where(t => t.Nombre.ToLower() == especialidadDestino.ToLower())
                        .Select(t => t.Nombre)
                        .FirstOrDefaultAsync();
                    if (tipoPrestacionValida != null) tipoPrestacionKey = tipoPrestacionValida;
                }
                var tipoPrestacionId = await _context.TIPO_PRESTACION
                    .Where(t => t.Nombre == tipoPrestacionKey)
                    .Select(t => t.Id)
                    .FirstOrDefaultAsync();

                if (diagnosticoId == 0 || procedenciaId == 0 || tipoPrestacionId == 0)
                    return 0;

                var solicitud = new SolicitudQuirurgicaReal
                {
                    ConsentimientoId = consentimientoId.Value,
                    DiagnosticoId = diagnosticoId,
                    ProcedenciaId = procedenciaId,
                    TipoPrestacionId = tipoPrestacionId,
                    FechaCreacion = DateTime.Now,
                    ValidacionGES = esGes,
                    ValidacionDuplicado = false,
                    IdSIGTE = true
                };
                _context.SOLICITUD_QUIRURGICA.Add(solicitud);
                await _context.SaveChangesAsync();

                var detallePaciente = new DetallePacienteReal
                {
                    SolicitudConsentimientoId = consentimientoId.Value,
                    SolicitudId = solicitud.IdSolicitud,
                    Peso = peso,
                    Altura = talla,
                    IMC = imc
                };
                _context.DETALLE_PACIENTE.Add(detallePaciente);

                var detalleClinico = new DetalleClinicoReal
                {
                    SolicitudConsentimientoId = consentimientoId.Value,
                    SolicitudId = solicitud.IdSolicitud,
                    TiempoEstimadoCirugia = tiempoEstimado,
                    EvaluacionAnestesica = evaluacionAnestesica,
                    EvaluacionTransfusion = evaluacionTransfusion
                };
                _context.DETALLE_CLINICO.Add(detalleClinico);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return solicitud.IdSolicitud;
            }
            catch
            {
                await transaction.RollbackAsync();
                return 0;
            }
        }

        // ✅ NUEVO: Obtener solicitudes por médico (por ahora retorna todas las solicitudes)
        public async Task<IEnumerable<SolicitudMedicoDto>> ObtenerSolicitudesPorMedicoAsync(int idMedico)
        {
            try
            {
                Console.WriteLine($"🔍 [Service] Obteniendo solicitudes del médico {idMedico}");

                // 📝 NOTA: Por ahora retorna TODAS las solicitudes porque no hay tabla MEDICO
                // TODO: Cuando implementes usuarios/médicos, agregar filtro WHERE
                var solicitudes = await _context.SOLICITUD_QUIRURGICA
                    .Include(s => s.Consentimiento)
                        .ThenInclude(c => c.Paciente)
                    .Include(s => s.Consentimiento)
                        .ThenInclude(c => c.Procedimiento)
                    .Include(s => s.Diagnostico)
                    .OrderByDescending(s => s.FechaCreacion)
                    .ToListAsync();

                var resultado = solicitudes.Select(s => new SolicitudMedicoDto
                {
                    Id = s.IdSolicitud,
                    NombrePaciente = $"{s.Consentimiento?.Paciente?.PrimerNombre ?? ""} {s.Consentimiento?.Paciente?.ApellidoPaterno ?? ""}".Trim(),
                    Rut = FormatearRut(s.Consentimiento?.Paciente?.Rut ?? "", s.Consentimiento?.Paciente?.Dv ?? ""),
                    Diagnostico = s.Diagnostico?.Nombre ?? "Sin diagnóstico",
                    Procedimiento = s.Consentimiento?.Procedimiento?.Nombre ?? "Sin procedimiento",
                    Estado = (s.ValidacionGES.HasValue && s.ValidacionGES.Value) ? "Priorizada" : "Pendiente",
                    FechaCreacion = s.FechaCreacion,
                    FechaProgramada = null, // TODO: Cuando exista la tabla PROGRAMACION_QUIRURGICA
                    DiasRestantes = null,
                    Contactabilidad = "Por Contactar" // TODO: Cuando exista esta información
                }).ToList();

                Console.WriteLine($"✅ Solicitudes obtenidas: {resultado.Count}");
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en ObtenerSolicitudesPorMedicoAsync: {ex.Message}");
                return new List<SolicitudMedicoDto>();
            }
        }

        // 🔹 MÉTODO SIMPLIFICADO 1: Solo datos básicos (CON DATOS REALES O DUMMY)
        public async Task<IEnumerable<SolicitudRecienteDto>> GetSolicitudesRecientesAsync()
        {
            try
            {
                // Intentar obtener datos REALES primero
                var solicitudesReales = await _context.SOLICITUD_QUIRURGICA
                    .Include(s => s.Consentimiento)
                    .ThenInclude(c => c.Paciente)
                    .Include(s => s.Consentimiento)
                    .ThenInclude(c => c.Procedimiento)
                    .OrderByDescending(s => s.FechaCreacion)
                    .Take(10)
                    .ToListAsync();

                var resultado = new List<SolicitudRecienteDto>();

                foreach (var solicitud in solicitudesReales)
                {



                    var esGes = solicitud.ValidacionGES.HasValue && solicitud.ValidacionGES.Value;
                    var dto = new SolicitudRecienteDto
                    {
                        SolicitudId = solicitud.IdSolicitud,
                        PacienteNombreCompleto = $"{solicitud.Consentimiento?.Paciente?.PrimerNombre} {solicitud.Consentimiento?.Paciente?.ApellidoPaterno}",
                        PacienteRut = solicitud.Consentimiento?.Paciente?.Rut ?? "N/A",
                        Prioridad = esGes ? "Prioritaria" : "Intermedia",
                        PrioridadCssClass = esGes ? "bg-success" : "bg-warning text-dark",
                        EsGes = esGes,
                        DescripcionProcedimiento = solicitud.Consentimiento?.Procedimiento?.Nombre ?? "Procedimiento no especificado",
                        FechaCreacion = solicitud.FechaCreacion,
                        TiempoTranscurrido = CalculateTimeAgo(solicitud.FechaCreacion)
                    };
                    resultado.Add(dto);
                }

                // Si no hay datos reales, usar dummy
                if (!resultado.Any())
                {
                    resultado = GetSolicitudesDummy();
                }

                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetSolicitudesRecientesAsync: {ex.Message}");
                // Fallback a datos dummy
                return GetSolicitudesDummy();
            }
        }

        // 🔹 MÉTODO SIMPLIFICADO 2: SOLO DATOS DUMMY (sin PROGRAMACION_QUIRURGICA)
        public async Task<IEnumerable<FechaProgramadaDto>> GetProximasFechasProgramadasAsync()
        {
            try
            {
                // ❌ ELIMINADA la consulta a PROGRAMACION_QUIRURGICA que no existe
                // ✅ SOLO datos dummy por ahora
                return GetFechasProgramadasDummy();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetProximasFechasProgramadasAsync: {ex.Message}");
                return GetFechasProgramadasDummy();
            }
        }

        // 🔹 MÉTODOS AUXILIARES PARA DATOS DUMMY
        private List<SolicitudRecienteDto> GetSolicitudesDummy()
        {
            return new List<SolicitudRecienteDto>
            {
                new SolicitudRecienteDto
                {
                    SolicitudId = 1,
                    PacienteNombreCompleto = "Juan Pérez González",
                    PacienteRut = "12.345.678-9",
                    Prioridad = "Prioritaria",
                    PrioridadCssClass = "bg-success",
                    EsGes = true,
                    DescripcionProcedimiento = "Cirugía de cadera",
                    FechaCreacion = DateTime.Now.AddDays(-2),
                    TiempoTranscurrido = "Hace 2 días"
                },
                new SolicitudRecienteDto
                {
                    SolicitudId = 2,
                    PacienteNombreCompleto = "María López Silva",
                    PacienteRut = "98.765.432-1",
                    Prioridad = "Intermedia",
                    PrioridadCssClass = "bg-warning text-dark",
                    EsGes = false,
                    DescripcionProcedimiento = "Artroscopía rodilla",
                    FechaCreacion = DateTime.Now.AddDays(-5),
                    TiempoTranscurrido = "Hace 5 días"
                }
            };
        }

        private List<FechaProgramadaDto> GetFechasProgramadasDummy()
        {
            return new List<FechaProgramadaDto>
            {
                new FechaProgramadaDto
                {
                    ProgramacionId = 1,
                    FechaProgramada = DateTime.Today.AddDays(1),
                    FechaProgramadaFormateada = DateTime.Today.AddDays(1).ToString("dd MMMM yyyy"),
                    PacienteNombreCompleto = "Paciente Demo 1",
                    DescripcionProcedimiento = "Procedimiento de prueba",
                    EsGes = true,
                    HoraProgramada = TimeSpan.FromHours(9),
                    Pabellon = "Pabellón Central"
                },
                new FechaProgramadaDto
                {
                    ProgramacionId = 2,
                    FechaProgramada = DateTime.Today.AddDays(3),
                    FechaProgramadaFormateada = DateTime.Today.AddDays(3).ToString("dd MMMM yyyy"),
                    PacienteNombreCompleto = "Paciente Demo 2",
                    DescripcionProcedimiento = "Cirugía programada",
                    EsGes = false,
                    HoraProgramada = TimeSpan.FromHours(11),
                    Pabellon = "Pabellón Norte"
                }
            };
        }

        // 🔹 FUNCIÓN AUXILIAR
        private string CalculateTimeAgo(DateTime date)
        {
            var diff = DateTime.Now - date;
            if (diff.TotalDays < 1) return $"Hace {(int)diff.TotalHours} horas";
            if (diff.TotalDays < 30) return $"Hace {(int)diff.TotalDays} días";
            return date.ToString("dd/MM/yyyy");
        }

        // 🔹 FUNCIÓN AUXILIAR PARA FORMATEAR RUT
        private string FormatearRut(string rut, string dv)
        {
            if (string.IsNullOrEmpty(rut)) return "N/A";
            return $"{rut}-{dv}";
        }

        // Para priorizar

        public async Task<bool> GuardarPriorizacionAsync(PriorizacionDto priorizacion)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1) Obtener la solicitud por Id (parte 1 de la PK)
                var solicitud = await _context.SOLICITUD_QUIRURGICA
                    .FirstOrDefaultAsync(s => s.IdSolicitud == priorizacion.SolicitudId);

                if (solicitud == null)
                {
                    Console.WriteLine($"❌ Solicitud {priorizacion.SolicitudId} no encontrada");
                    return false;
                }

                // 2) Buscar el criterio por ID (lo envía el front)
                var criterio = await _context.CRITERIO_PRIORIZACION
                    .FirstOrDefaultAsync(c => c.Id == priorizacion.CriterioPriorizacionId);

                if (criterio == null)
                {
                    Console.WriteLine($"❌ CriterioPriorizacionId {priorizacion.CriterioPriorizacionId} no existe");
                    return false;
                }

                // 3) Calcular prioridad numérica según el NOMBRE del criterio
                var prioridadNumerica = CalcularPrioridadNumerica(criterio.Nombre);

                // 4) Marcar solicitud como priorizada (tu lógica actual)
                solicitud.ValidacionGES = true;
                _context.SOLICITUD_QUIRURGICA.Update(solicitud);

                // 5) Crear la priorización (⚠️ incluir ambas columnas de la FK compuesta)
                var nuevaPriorizacion = new PriorizacionSolicitud
                {
                    SolicitudQuirurgicaId = solicitud.IdSolicitud,     // parte 1
                    SolicitudConsentimientoId = solicitud.ConsentimientoId, // parte 2
                    CriterioPriorizacionId = criterio.Id,
                    Prioridad = (byte)prioridadNumerica,
                    FechaPriorizacion = priorizacion.FechaPriorizacion,
                    MotivoPriorizacionId = null // opcional
                };

                //_context.PRIORIZACION_SOLICITUD.Add(nuevaPriorizacion);
                // await _context.SaveChangesAsync();



                await _context.SaveChangesAsync();

                // 5.2) Inserta la priorización vía SP (evita OUTPUT y no choca con triggers)
                await _context.Database.ExecuteSqlRawAsync(@"
                EXEC dbo.usp_PRIORIZACION_SOLICITUD_Insert
                     @CRITERIO_PRIORIZACION_id = {0},
                     @fechaPriorizacion = {1},
                     @MOTIVO_PRIORIZACION_id = {2},
                     @prioridad = {3},
                     @SOLICITUD_QUIRURGICA_CONSENTIMIENTO_INFORMADO_id = {4},
                     @SOLICITUD_QUIRURGICA_idSolicitud = {5}
            ",
                    nuevaPriorizacion.CriterioPriorizacionId,
                    nuevaPriorizacion.FechaPriorizacion,
                    nuevaPriorizacion.MotivoPriorizacionId,
                    nuevaPriorizacion.Prioridad,
                    nuevaPriorizacion.SolicitudConsentimientoId,
                    nuevaPriorizacion.SolicitudQuirurgicaId
                );



                await transaction.CommitAsync();

                Console.WriteLine($"✅ Solicitud {priorizacion.SolicitudId} priorizada con criterio '{criterio.Nombre}' (P{prioridadNumerica})");
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"❌ Error en GuardarPriorizacionAsync: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");
                return false;
            }
        }

        // ✅ NUEVO MÉTODO: Calcular prioridad numérica
        private int CalcularPrioridadNumerica(string criterioNombre)
        {
            switch ((criterioNombre ?? string.Empty).Trim().ToLower())
            {
                // PRIORIDAD 1 - URGENTE
                case "patología ges":
                case "prioridad sanitaria":
                case "ges":
                case "sanitaria":
                    return 1;

                // PRIORIDAD 2 - ALTA
                case "prais":
                case "sename":
                case "comges":
                case "licencia médica prolongada":
                case "licencia":
                    return 2;

                // PRIORIDAD 3 - MEDIA
                case "percentil 50 más antiguo":
                case "percentil50":
                case "oportunidad logística":
                case "logistica":
                    return 3;

                default:
                    return 3;
            }
        }

        private string MapearCriterioANombre(string criterioKey)
        {
            return criterioKey.ToLower() switch
            {
                "ges" => "Patología GES",
                "prais" => "PRAIS",
                "sename" => "SENAME",
                "comges" => "COMGES",
                "sanitaria" => "Prioridad Sanitaria",
                "logistica" => "Oportunidad Logística",
                "percentil50" => "Percentil 50 más antiguo",
                "licencia" => "Licencia médica prolongada",
                _ => "Otro"
            };
        }



        public async Task<IEnumerable<SolicitudMedicoDto>> ObtenerSolicitudesPendientesAsync()
        {
            try
            {
                var solicitudes = await _context.SOLICITUD_QUIRURGICA
                    .Include(s => s.Consentimiento).ThenInclude(c => c.Paciente)
                    .Include(s => s.Consentimiento).ThenInclude(c => c.Procedimiento)
                    .Include(s => s.Diagnostico)
                    .Where(s => !s.ValidacionGES.HasValue || !s.ValidacionGES.Value)
                    .OrderBy(s => s.FechaCreacion)
                    .ToListAsync();

                return solicitudes.Select(s => new SolicitudMedicoDto
                {
                    Id = s.IdSolicitud,
                    NombrePaciente = $"{s.Consentimiento?.Paciente?.PrimerNombre ?? ""} {s.Consentimiento?.Paciente?.ApellidoPaterno ?? ""}".Trim(),
                    Rut = FormatearRut(s.Consentimiento?.Paciente?.Rut ?? "", s.Consentimiento?.Paciente?.Dv ?? ""),
                    Diagnostico = s.Diagnostico?.Nombre ?? "Sin diagnóstico",
                    Procedimiento = s.Consentimiento?.Procedimiento?.Nombre ?? "Sin procedimiento",
                    Estado = "Pendiente",
                    FechaCreacion = s.FechaCreacion
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
                return new List<SolicitudMedicoDto>();
            }
        }




        public async Task<IEnumerable<SolicitudMedicoDto>> ObtenerSolicitudesPriorizadasAsync()
        {
            try
            {
                var solicitudes = await _context.SOLICITUD_QUIRURGICA
                    .Include(s => s.Consentimiento).ThenInclude(c => c.Paciente)
                    .Include(s => s.Consentimiento).ThenInclude(c => c.Procedimiento)
                    .Include(s => s.Diagnostico)
                    .Where(s => s.ValidacionGES.HasValue && s.ValidacionGES.Value)
                    .OrderByDescending(s => s.FechaCreacion)
                    .ToListAsync();

                // Para cada solicitud priorizada, obtener la prioridad (P1/P2/P3) desde PRIORIZACION_SOLICITUD
                var resultado = solicitudes.Select(s => new SolicitudMedicoDto
                {
                    Id = s.IdSolicitud,
                    NombrePaciente = $"{s.Consentimiento?.Paciente?.PrimerNombre ?? ""} {s.Consentimiento?.Paciente?.ApellidoPaterno ?? ""}".Trim(),
                    Rut = FormatearRut(s.Consentimiento?.Paciente?.Rut ?? "", s.Consentimiento?.Paciente?.Dv ?? ""),
                    Diagnostico = s.Diagnostico?.Nombre ?? "Sin diagnóstico",
                    Procedimiento = s.Consentimiento?.Procedimiento?.Nombre ?? "Sin procedimiento",
                    Estado = "Priorizada",
                    FechaCreacion = s.FechaCreacion,
                    Prioridad = (byte?)_context.PRIORIZACION_SOLICITUD
                                .Where(p => p.SolicitudQuirurgicaId == s.IdSolicitud && p.SolicitudConsentimientoId == s.ConsentimientoId)
                                .OrderByDescending(p => p.FechaPriorizacion)
                                .Select(p => p.Prioridad)
                                .FirstOrDefault() ?? (byte)3
                }).ToList();

                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
                return new List<SolicitudMedicoDto>();
            }
        }




        public async Task<SolicitudDetalleDto?> ObtenerSolicitudDetalleAsync(int solicitudId)
{
    try
    {
        var solicitud = await _context.SOLICITUD_QUIRURGICA
            .Include(s => s.Consentimiento).ThenInclude(c => c.Paciente)
            .Include(s => s.Consentimiento).ThenInclude(c => c.Procedimiento)
            .Include(s => s.Diagnostico)
            .Include(s => s.Procedencia)
            .Include(s => s.DetallesPaciente)
            .Include(s => s.DetalleClinico)
            .FirstOrDefaultAsync(s => s.IdSolicitud == solicitudId);

        if (solicitud == null) return null;

        var detallePaciente = solicitud.DetallesPaciente?.FirstOrDefault();
                var detalleClinico = solicitud.DetalleClinico;

        return new SolicitudDetalleDto
        {
            Id = solicitud.IdSolicitud,
            NombrePaciente = $"{solicitud.Consentimiento?.Paciente?.PrimerNombre ?? ""} {solicitud.Consentimiento?.Paciente?.ApellidoPaterno ?? ""}".Trim(),
            Rut = FormatearRut(solicitud.Consentimiento?.Paciente?.Rut ?? "", solicitud.Consentimiento?.Paciente?.Dv ?? ""),
            Diagnostico = solicitud.Diagnostico?.Nombre ?? "Sin diagnóstico",
            Procedimiento = solicitud.Consentimiento?.Procedimiento?.Nombre ?? "Sin procedimiento",
            Procedencia = solicitud.Procedencia?.Nombre ?? "N/A",
            EsGes = solicitud.ValidacionGES ?? false,
            Estado = (solicitud.ValidacionGES ?? false) ? "Priorizada" : "Pendiente",
            FechaCreacion = solicitud.FechaCreacion,
            Peso = detallePaciente?.Peso,
            Talla = detallePaciente?.Altura,
            IMC = detallePaciente?.IMC,
            TiempoEstimado = detalleClinico?.TiempoEstimadoCirugia,
            EvaluacionAnestesica = detalleClinico?.EvaluacionAnestesica ?? false,
            EvaluacionTransfusion = detalleClinico?.EvaluacionTransfusion ?? false
        };
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error: {ex.Message}");
        return null;
    }
}



    }
}