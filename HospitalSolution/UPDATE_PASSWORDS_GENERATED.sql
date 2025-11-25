-- =============================================
-- SCRIPT GENERADO AUTOMÁTICAMENTE
-- Fecha: 2025-11-25 19:00:55
-- =============================================

USE [HospitalV4];
GO

-- Ver usuarios actuales
SELECT id, username, rol, activo, 
       LEFT(password_hash, 30) + '...' AS hash_preview
FROM [dbo].[USUARIO]
ORDER BY rol, username;
GO

-- Actualizar contraseña de administradores a "admin"
UPDATE [dbo].[USUARIO]
SET password_hash = ''
WHERE rol = 'Administrador';
GO

-- Actualizar contraseña de médicos a "medico"
UPDATE [dbo].[USUARIO]
SET password_hash = ''
WHERE rol = 'Medico';
GO

-- Verificar cambios
SELECT id, username, rol, activo,
       LEFT(password_hash, 30) + '...' AS hash_nuevo
FROM [dbo].[USUARIO]
ORDER BY rol, username;
GO

-- =============================================
-- CREDENCIALES ACTUALIZADAS:
-- =============================================
-- admin / admin2  → Contraseña: admin  (Rol: Administrador)
-- medico1 / medico2 → Contraseña: medico (Rol: Medico)
-- =============================================
