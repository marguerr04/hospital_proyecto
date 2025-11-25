# =============================================
# SCRIPT AUTOMÁTICO: GENERAR HASHES BCRYPT
# =============================================
# Este script genera los hashes para "admin" y "medico"
# y te da los comandos SQL listos para copiar

Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  GENERADOR DE HASHES BCRYPT" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""
a
# Verificar que la API esté corriendo
Write-Host "[1/4] Verificando que la API esté corriendo..." -ForegroundColor Yellow

try {
    $response = Invoke-RestMethod -Uri "https://localhost:7032/api/Auth/test" -Method Get -SkipCertificateCheck
    Write-Host "✅ API está funcionando. Usuarios en BD: $($response.usuariosEnBD)" -ForegroundColor Green
    Write-Host ""
}
catch {
    Write-Host "❌ ERROR: La API no está corriendo en https://localhost:7032" -ForegroundColor Red
    Write-Host ""
    Write-Host "Por favor, ejecuta primero:" -ForegroundColor Yellow
    Write-Host "  cd E:\Retorno_cero\Testeo_recuperacion_1\Hospital.Api" -ForegroundColor White
    Write-Host "  dotnet run" -ForegroundColor White
    Write-Host ""
    exit
}

# Generar hash para "admin"
Write-Host "[2/4] Generando hash para contraseña 'admin'..." -ForegroundColor Yellow

$bodyAdmin = '"admin"' | ConvertTo-Json
try {
    $hashAdmin = Invoke-RestMethod -Uri "https://localhost:7032/api/Auth/generate-hash" `
        -Method Post `
        -Body $bodyAdmin `
        -ContentType "application/json" `
        -SkipCertificateCheck
    
    Write-Host "✅ Hash generado para 'admin'" -ForegroundColor Green
    $hashAdminValue = $hashAdmin.hash
}
catch {
    Write-Host "❌ ERROR al generar hash para 'admin': $_" -ForegroundColor Red
    exit
}

Write-Host ""

# Generar hash para "medico"
Write-Host "[3/4] Generando hash para contraseña 'medico'..." -ForegroundColor Yellow

$bodyMedico = '"medico"' | ConvertTo-Json
try {
    $hashMedico = Invoke-RestMethod -Uri "https://localhost:7032/api/Auth/generate-hash" `
        -Method Post `
        -Body $bodyMedico `
        -ContentType "application/json" `
        -SkipCertificateCheck
    
    Write-Host "✅ Hash generado para 'medico'" -ForegroundColor Green
    $hashMedicoValue = $hashMedico.hash
}
catch {
    Write-Host "❌ ERROR al generar hash para 'medico': $_" -ForegroundColor Red
    exit
}

Write-Host ""
Write-Host ""

# Mostrar resultados
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  HASHES GENERADOS EXITOSAMENTE" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "📋 HASH PARA 'admin':" -ForegroundColor Yellow
Write-Host $hashAdminValue -ForegroundColor White
Write-Host ""

Write-Host "📋 HASH PARA 'medico':" -ForegroundColor Yellow
Write-Host $hashMedicoValue -ForegroundColor White
Write-Host ""
Write-Host ""

# Generar script SQL
Write-Host "[4/4] Generando script SQL..." -ForegroundColor Yellow
Write-Host ""

$sqlScript = @"
-- =============================================
-- SCRIPT GENERADO AUTOMÁTICAMENTE
-- Fecha: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
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
SET password_hash = '$hashAdminValue'
WHERE rol = 'Administrador';
GO

-- Actualizar contraseña de médicos a "medico"
UPDATE [dbo].[USUARIO]
SET password_hash = '$hashMedicoValue'
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
"@

# Guardar SQL en archivo
$sqlScript | Out-File -FilePath "UPDATE_PASSWORDS_GENERATED.sql" -Encoding UTF8

Write-Host "✅ Script SQL guardado en: UPDATE_PASSWORDS_GENERATED.sql" -ForegroundColor Green
Write-Host ""
Write-Host ""

# Mostrar instrucciones
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  SIGUIENTE PASO" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. Abre SQL Server Management Studio" -ForegroundColor Yellow
Write-Host "2. Conéctate a tu servidor: localhost\MSSQLSERVER01" -ForegroundColor Yellow
Write-Host "3. Abre el archivo: UPDATE_PASSWORDS_GENERATED.sql" -ForegroundColor Yellow
Write-Host "4. Ejecuta el script completo (F5)" -ForegroundColor Yellow
Write-Host "5. Verifica que se actualizaron los registros" -ForegroundColor Yellow
Write-Host ""
Write-Host "Luego podrás hacer login con:" -ForegroundColor Green
Write-Host "  • Usuario: admin    | Contraseña: admin  | Rol: Administrador" -ForegroundColor White
Write-Host "  • Usuario: medico1  | Contraseña: medico | Rol: Medico" -ForegroundColor White
Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""
