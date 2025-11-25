-- =============================================
-- SCRIPT DE ACTUALIZACIÓN DE CONTRASEÑAS
-- Base de datos: HospitalV4
-- Tabla: dbo.USUARIO
-- =============================================

-- IMPORTANTE: Primero genera los hashes usando la API
-- Endpoint: POST https://localhost:7032/api/Auth/generate-hash
-- Body: "admin" o "medico"

-- =============================================
-- PASO 1: VER USUARIOS ACTUALES
-- =============================================
SELECT 
    id,
    username,
    rol,
    activo,
    LEFT(password_hash, 30) + '...' AS hash_actual_preview
FROM [HospitalV4].[dbo].[USUARIO]
ORDER BY rol, username;

-- =============================================
-- PASO 2: ACTUALIZAR CONTRASEÑAS
-- =============================================

-- REEMPLAZA '<HASH_ADMIN>' con el hash generado para "admin"
-- REEMPLAZA '<HASH_MEDICO>' con el hash generado para "medico"

-- Actualizar administradores
UPDATE [HospitalV4].[dbo].[USUARIO]
SET password_hash = '<HASH_ADMIN>'
WHERE rol = 'Administrador';

-- Actualizar médicos
UPDATE [HospitalV4].[dbo].[USUARIO]
SET password_hash = '<HASH_MEDICO>'
WHERE rol = 'Medico';

-- =============================================
-- PASO 3: VERIFICAR CAMBIOS
-- =============================================
SELECT 
    id,
    username,
    rol,
    activo,
    LEFT(password_hash, 30) + '...' AS hash_nuevo_preview
FROM [HospitalV4].[dbo].[USUARIO]
ORDER BY rol, username;

-- =============================================
-- ROLLBACK (si algo sale mal)
-- =============================================
-- Si necesitas revertir, ejecuta:
-- UPDATE [HospitalV4].[dbo].[USUARIO]
-- SET password_hash = '<HASH_ANTERIOR>'
-- WHERE id = <ID_DEL_USUARIO>;
