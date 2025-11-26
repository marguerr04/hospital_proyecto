# =============================================
# Script PowerShell de Verificación Automática
# Hospital Padre Hurtado
# Fecha: 2025-01-22
# =============================================

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "?? VERIFICACIÓN AUTOMÁTICA DEL PROYECTO" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Función para verificar si un puerto está en uso
function Test-Port {
    param (
        [int]$Port
    )
    $connection = Test-NetConnection -ComputerName localhost -Port $Port -InformationLevel Quiet
    return $connection
}

# 1. Verificar que la API esté corriendo
Write-Host "?? VERIFICACIÓN 1: API (Puerto 7032)" -ForegroundColor Yellow
if (Test-Port -Port 7032) {
    Write-Host "? API está corriendo en https://localhost:7032" -ForegroundColor Green
} else {
    Write-Host "? API NO está corriendo" -ForegroundColor Red
    Write-Host "   Ejecuta: cd Hospital.Api && dotnet run" -ForegroundColor Gray
}
Write-Host ""

# 2. Verificar que Blazor esté corriendo
Write-Host "?? VERIFICACIÓN 2: Blazor (Puerto 7213)" -ForegroundColor Yellow
if (Test-Port -Port 7213) {
    Write-Host "? Blazor está corriendo en https://localhost:7213" -ForegroundColor Green
} else {
    Write-Host "? Blazor NO está corriendo" -ForegroundColor Red
    Write-Host "   Ejecuta: cd proyecto_hospital_version_1 && dotnet run" -ForegroundColor Gray
}
Write-Host ""

# 3. Verificar archivos SQL
Write-Host "?? VERIFICACIÓN 3: Scripts SQL" -ForegroundColor Yellow
$archivosSql = @(
    "SQL_POBLAR_PROFESIONALES.sql",
    "SQL_VERIFICACION_COMPLETA.sql"
)

foreach ($archivo in $archivosSql) {
    if (Test-Path $archivo) {
        Write-Host "? $archivo existe" -ForegroundColor Green
    } else {
        Write-Host "? $archivo NO encontrado" -ForegroundColor Red
    }
}
Write-Host ""

# 4. Verificar archivos de código (API)
Write-Host "?? VERIFICACIÓN 4: Archivos de API" -ForegroundColor Yellow
$archivosApi = @(
    "..\Hospital.Api\DTOs\AuditoriaPriorizacionDto.cs",
    "..\Hospital.Api\Data\Services\AuditoriaPriorizacionService.cs",
    "..\Hospital.Api\Controllers\AuditoriaPriorizacionController.cs"
)

foreach ($archivo in $archivosApi) {
    if (Test-Path $archivo) {
        Write-Host "? $archivo existe" -ForegroundColor Green
    } else {
        Write-Host "? $archivo NO encontrado" -ForegroundColor Red
    }
}
Write-Host ""

# 5. Verificar archivos de código (Blazor)
Write-Host "?? VERIFICACIÓN 5: Archivos de Blazor" -ForegroundColor Yellow
$archivosBlazor = @(
    "..\proyecto_hospital_version_1\Services\AuditoriaPriorizacionApiService.cs",
    "..\proyecto_hospital_version_1\Components\Pages\HistorialPriorizaciones.razor"
)

foreach ($archivo in $archivosBlazor) {
    if (Test-Path $archivo) {
        Write-Host "? $archivo existe" -ForegroundColor Green
    } else {
        Write-Host "? $archivo NO encontrado" -ForegroundColor Red
    }
}
Write-Host ""

# 6. Verificar documentación
Write-Host "?? VERIFICACIÓN 6: Documentación" -ForegroundColor Yellow
$archivosDoc = @(
    "GUIA_HISTORIAL_LOGS_COMPLETA.md",
    "RESUMEN_IMPLEMENTACION_LOGS.md",
    "INDICE_ARCHIVOS_LOGS.md"
)

foreach ($archivo in $archivosDoc) {
    if (Test-Path $archivo) {
        Write-Host "? $archivo existe" -ForegroundColor Green
    } else {
        Write-Host "? $archivo NO encontrado" -ForegroundColor Red
    }
}
Write-Host ""

# 7. Intentar llamar al endpoint de la API
Write-Host "?? VERIFICACIÓN 7: Endpoint de Auditoría" -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "https://localhost:7032/api/AuditoriaPriorizacion?pageNumber=1&pageSize=5" -Method GET -SkipCertificateCheck -UseBasicParsing -ErrorAction Stop
    if ($response.StatusCode -eq 200) {
        Write-Host "? Endpoint funcionando correctamente" -ForegroundColor Green
        Write-Host "   Status Code: $($response.StatusCode)" -ForegroundColor Gray
    }
} catch {
    Write-Host "? No se pudo conectar al endpoint" -ForegroundColor Red
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Gray
}
Write-Host ""

# 8. Resumen final
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "?? RESUMEN DE VERIFICACIÓN" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

$totalArchivos = $archivosSql.Count + $archivosApi.Count + $archivosBlazor.Count + $archivosDoc.Count
$archivosEncontrados = 0

foreach ($archivo in ($archivosSql + $archivosApi + $archivosBlazor + $archivosDoc)) {
    if (Test-Path $archivo) {
        $archivosEncontrados++
    }
}

Write-Host "Archivos verificados: $archivosEncontrados / $totalArchivos" -ForegroundColor White
Write-Host ""

if (Test-Port -Port 7032) {
    Write-Host "? API: Running" -ForegroundColor Green
} else {
    Write-Host "? API: Not Running" -ForegroundColor Red
}

if (Test-Port -Port 7213) {
    Write-Host "? Blazor: Running" -ForegroundColor Green
} else {
    Write-Host "? Blazor: Not Running" -ForegroundColor Red
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "?? VERIFICACIÓN COMPLETADA" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Instrucciones finales
Write-Host "?? PRÓXIMOS PASOS:" -ForegroundColor Yellow
Write-Host "   1. Si la API no está corriendo: cd Hospital.Api && dotnet run" -ForegroundColor Gray
Write-Host "   2. Si Blazor no está corriendo: cd proyecto_hospital_version_1 && dotnet run" -ForegroundColor Gray
Write-Host "   3. Ejecutar script SQL: SQL_POBLAR_PROFESIONALES.sql" -ForegroundColor Gray
Write-Host "   4. Navegar a: https://localhost:7213/dashboard" -ForegroundColor Gray
Write-Host "   5. Click en 'Historial de Logs'" -ForegroundColor Gray
Write-Host ""

Read-Host "Presiona Enter para cerrar"
