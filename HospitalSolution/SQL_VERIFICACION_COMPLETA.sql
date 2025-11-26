-- =============================================
-- SCRIPT DE VERIFICACIÓN COMPLETA
-- Hospital Padre Hurtado
-- Fecha: 2025-01-22
-- =============================================

USE HospitalV4;
GO

PRINT '========================================';
PRINT '?? VERIFICACIÓN 1: TABLA PROFESIONAL';
PRINT '========================================';
GO

-- Contar total de profesionales
SELECT COUNT(*) AS TotalProfesionales 
FROM [dbo].[PROFESIONAL];
GO

-- Ver los últimos 10 profesionales insertados
SELECT TOP 10
    Id,
    rut + '-' + dv AS RUT,
    primerNombre + ' ' + ISNULL(segundoNombre + ' ', '') + 
    primerApellido + ' ' + ISNULL(segundoApellido, '') AS NombreCompleto
FROM [dbo].[PROFESIONAL]
ORDER BY Id DESC;
GO

PRINT '';
PRINT '========================================';
PRINT '?? VERIFICACIÓN 2: TABLA AUDITORÍA';
PRINT '========================================';
GO

-- Contar total de registros de auditoría
SELECT COUNT(*) AS TotalRegistrosAuditoria 
FROM [dbo].[AUD_PRIORIZACION_SOLICITUD];
GO

-- Ver los últimos 10 registros de auditoría
SELECT TOP 10
    AudId,
    AudFecha,
    AudAccion,
    AudUsuario,
    SOLICITUD_QUIRURGICA_idSolicitud AS SolicitudID,
    prioridad AS Prioridad
FROM [dbo].[AUD_PRIORIZACION_SOLICITUD]
ORDER BY AudFecha DESC;
GO

PRINT '';
PRINT '========================================';
PRINT '?? VERIFICACIÓN 3: DISTRIBUCIÓN DE PRIORIDADES';
PRINT '========================================';
GO

-- Ver distribución de prioridades en la auditoría
SELECT 
    prioridad AS Prioridad,
    COUNT(*) AS Cantidad,
    CASE 
        WHEN prioridad = 1 THEN 'URGENTE (P1)'
        WHEN prioridad = 2 THEN 'ALTA (P2)'
        WHEN prioridad = 3 THEN 'MEDIA (P3)'
        ELSE 'OTRA'
    END AS Descripcion
FROM [dbo].[AUD_PRIORIZACION_SOLICITUD]
GROUP BY prioridad
ORDER BY prioridad;
GO

PRINT '';
PRINT '========================================';
PRINT '?? VERIFICACIÓN 4: ACTIVIDAD POR USUARIO';
PRINT '========================================';
GO

-- Ver actividad por usuario
SELECT 
    AudUsuario AS Usuario,
    COUNT(*) AS TotalAcciones,
    MIN(AudFecha) AS PrimeraAccion,
    MAX(AudFecha) AS UltimaAccion
FROM [dbo].[AUD_PRIORIZACION_SOLICITUD]
GROUP BY AudUsuario
ORDER BY TotalAcciones DESC;
GO

PRINT '';
PRINT '========================================';
PRINT '?? VERIFICACIÓN 5: QUERY DE PAGINACIÓN (Simulación)';
PRINT '========================================';
GO

-- Simular la query que usa el endpoint de API
-- Página 1 (primeros 20 registros)
DECLARE @PageNumber INT = 1;
DECLARE @PageSize INT = 20;
DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

SELECT 
    [AudId],
    [AudFecha],
    [AudAccion],
    [AudUsuario],
    [id],
    [SOLICITUD_QUIRURGICA_idSolicitud],
    [prioridad]
FROM [dbo].[AUD_PRIORIZACION_SOLICITUD]
ORDER BY [AudFecha] DESC
OFFSET @Offset ROWS
FETCH NEXT @PageSize ROWS ONLY;
GO

PRINT '';
PRINT '========================================';
PRINT '? VERIFICACIÓN COMPLETA FINALIZADA';
PRINT '========================================';
GO

-- Resumen final
SELECT 
    'Profesionales' AS Tabla,
    COUNT(*) AS TotalRegistros
FROM [dbo].[PROFESIONAL]
UNION ALL
SELECT 
    'Auditoría Priorizaciones' AS Tabla,
    COUNT(*) AS TotalRegistros
FROM [dbo].[AUD_PRIORIZACION_SOLICITUD];
GO

PRINT '';
PRINT '?? Si ves resultados arriba, todo está funcionando correctamente!';
PRINT '';
