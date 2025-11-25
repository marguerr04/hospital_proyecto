# =============================================
# GENERADOR DE HASHES - VERSIÓN SIMPLIFICADA
# =============================================

Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  GENERADOR DE HASHES BCRYPT" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "IMPORTANTE: Asegúrate de que la API esté corriendo en https://localhost:7032" -ForegroundColor Yellow
Write-Host ""
Write-Host "Si aún no la has iniciado, abre otra terminal y ejecuta:" -ForegroundColor Yellow
Write-Host "  cd E:\Retorno_cero\Testeo_recuperacion_1\Hospital.Api" -ForegroundColor White
Write-Host "  dotnet run" -ForegroundColor White
Write-Host ""
Write-Host "Presiona ENTER cuando la API esté lista..."
$null = Read-Host

# Ignorar certificados SSL autofirmados
add-type @"
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    public class TrustAllCertsPolicy : ICertificatePolicy {
        public bool CheckValidationResult(
            ServicePoint srvPoint, X509Certificate certificate,
            WebRequest request, int certificateProblem) {
            return true;
        }
    }
"@
[System.Net.ServicePointManager]::CertificatePolicy = New-Object TrustAllCertsPolicy
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

Write-Host ""
Write-Host "[1/3] Probando conexión con la API..." -ForegroundColor Yellow

try {
    $testUrl = "https://localhost:7032/api/Auth/test"
    $response = Invoke-RestMethod -Uri $testUrl -Method Get
    Write-Host "? API funcionando. Usuarios en BD: $($response.usuariosEnBD)" -ForegroundColor Green
}
catch {
    Write-Host "? ERROR: No se pudo conectar con la API" -ForegroundColor Red
    Write-Host "Detalles: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "Verifica que la API esté corriendo correctamente." -ForegroundColor Yellow
    exit
}

Write-Host ""
Write-Host "[2/3] Generando hash para 'admin'..." -ForegroundColor Yellow

try {
    $url = "https://localhost:7032/api/Auth/generate-hash"
    $body = '"admin"'
    $response = Invoke-RestMethod -Uri $url -Method Post -Body $body -ContentType "application/json"
    $hashAdmin = $response.hash
    Write-Host "? Hash generado para 'admin'" -ForegroundColor Green
}
catch {
    Write-Host "? ERROR al generar hash para 'admin'" -ForegroundColor Red
    Write-Host "Detalles: $_" -ForegroundColor Red
    exit
}

Write-Host ""
Write-Host "[3/3] Generando hash para 'medico'..." -ForegroundColor Yellow

try {
    $url = "https://localhost:7032/api/Auth/generate-hash"
    $body = '"medico"'
    $response = Invoke-RestMethod -Uri $url -Method Post -Body $body -ContentType "application/json"
    $hashMedico = $response.hash
    Write-Host "? Hash generado para 'medico'" -ForegroundColor Green
}
catch {
    Write-Host "? ERROR al generar hash para 'medico'" -ForegroundColor Red
    Write-Host "Detalles: $_" -ForegroundColor Red
    exit
}

Write-Host ""
Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  HASHES GENERADOS EXITOSAMENTE" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "?? HASH PARA 'admin':" -ForegroundColor Yellow
Write-Host $hashAdmin -ForegroundColor White
Write-Host ""

Write-Host "?? HASH PARA 'medico':" -ForegroundColor Yellow
Write-Host $hashMedico -ForegroundColor White
Write-Host ""
Write-Host ""

# Generar script SQL
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
SET password_hash = '$hashAdmin'
WHERE rol = 'Administrador';
GO

-- Actualizar contraseña de médicos a "medico"
UPDATE [dbo].[USUARIO]
SET password_hash = '$hashMedico'
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
-- admin / admin2  ? Contraseña: admin  (Rol: Administrador)
-- medico1 / medico2 ? Contraseña: medico (Rol: Medico)
-- =============================================
"@

$sqlScript | Out-File -FilePath "UPDATE_PASSWORDS_GENERATED.sql" -Encoding UTF8

Write-Host "? Script SQL guardado en: UPDATE_PASSWORDS_GENERATED.sql" -ForegroundColor Green
Write-Host ""
Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  SIGUIENTE PASO" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. Abre SQL Server Management Studio" -ForegroundColor Yellow
Write-Host "2. Conéctate a: localhost\MSSQLSERVER01" -ForegroundColor Yellow
Write-Host "3. Abre el archivo: UPDATE_PASSWORDS_GENERATED.sql" -ForegroundColor Yellow
Write-Host "4. Ejecuta el script (F5)" -ForegroundColor Yellow
Write-Host ""
Write-Host "Credenciales resultantes:" -ForegroundColor Green
Write-Host "  • admin    / admin  / Administrador ? /dashboard" -ForegroundColor White
Write-Host "  • medico1  / medico / Medico        ? /home-medico" -ForegroundColor White
Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Presiona ENTER para salir..."
$null = Read-Host
