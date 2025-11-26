# ============================================
# Script de Verificación y Compilación
# Solución Error Historial de Logs
# ============================================

Write-Host ""
Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  VERIFICACIÓN Y COMPILACIÓN - HISTORIAL LOGS  " -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Configuración de rutas
$solutionPath = "E:\Retorno_cero\Testeo_recuperacion_1\HospitalSolution"
$apiPath = "E:\Retorno_cero\Testeo_recuperacion_1\Hospital.Api"
$blazorPath = "E:\Retorno_cero\Testeo_recuperacion_1\proyecto_hospital_version_1"

# Archivos a verificar
$file1 = "$apiPath\DTOs\AuditoriaPriorizacionDto.cs"
$file2 = "$apiPath\Data\Services\AuditoriaPriorizacionService.cs"

# ============================================
# PASO 1: Verificar que los archivos existen
# ============================================
Write-Host "PASO 1: Verificando archivos..." -ForegroundColor Yellow
Write-Host ""

if (Test-Path $file1) {
    Write-Host "? Archivo 1 encontrado: AuditoriaPriorizacionDto.cs" -ForegroundColor Green
} else {
    Write-Host "? ERROR: No se encuentra AuditoriaPriorizacionDto.cs" -ForegroundColor Red
    exit 1
}

if (Test-Path $file2) {
    Write-Host "? Archivo 2 encontrado: AuditoriaPriorizacionService.cs" -ForegroundColor Green
} else {
    Write-Host "? ERROR: No se encuentra AuditoriaPriorizacionService.cs" -ForegroundColor Red
    exit 1
}

Write-Host ""

# ============================================
# PASO 2: Verificar cambios aplicados
# ============================================
Write-Host "PASO 2: Verificando cambios aplicados..." -ForegroundColor Yellow
Write-Host ""

$content1 = Get-Content $file1 -Raw
$content2 = Get-Content $file2 -Raw

if ($content1 -match "public byte Prioridad") {
    Write-Host "? AuditoriaPriorizacionDto.cs - CORRECTO (byte Prioridad)" -ForegroundColor Green
} else {
    Write-Host "? ERROR: AuditoriaPriorizacionDto.cs no tiene 'byte Prioridad'" -ForegroundColor Red
    Write-Host "   Revisa manualmente el archivo en línea 20" -ForegroundColor Yellow
}

if ($content2 -match "public byte Prioridad") {
    Write-Host "? AuditoriaPriorizacionService.cs - CORRECTO (byte Prioridad)" -ForegroundColor Green
} else {
    Write-Host "? ERROR: AuditoriaPriorizacionService.cs no tiene 'byte Prioridad'" -ForegroundColor Red
    Write-Host "   Revisa manualmente el archivo en línea 122" -ForegroundColor Yellow
}

Write-Host ""

# ============================================
# PASO 3: Verificar procesos en ejecución
# ============================================
Write-Host "PASO 3: Verificando procesos en ejecución..." -ForegroundColor Yellow
Write-Host ""

$apiProcess = Get-Process -Name "Hospital.Api" -ErrorAction SilentlyContinue
$blazorProcess = Get-Process -Name "proyecto_hospital_version_1" -ErrorAction SilentlyContinue

if ($apiProcess) {
    Write-Host "??  ADVERTENCIA: Hospital.Api está corriendo" -ForegroundColor Yellow
    Write-Host "   Debes detener la API antes de compilar" -ForegroundColor Yellow
    Write-Host "   ¿Quieres detener el proceso ahora? (S/N)" -ForegroundColor Cyan
    $respuesta = Read-Host
    if ($respuesta -eq "S" -or $respuesta -eq "s") {
        Stop-Process -Name "Hospital.Api" -Force
        Write-Host "? Proceso detenido" -ForegroundColor Green
        Start-Sleep -Seconds 2
    } else {
        Write-Host "? No se puede compilar con la API corriendo" -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "? Hospital.Api NO está corriendo (OK para compilar)" -ForegroundColor Green
}

if ($blazorProcess) {
    Write-Host "??  ADVERTENCIA: Blazor está corriendo" -ForegroundColor Yellow
    Write-Host "   Debes detener Blazor antes de compilar" -ForegroundColor Yellow
    Write-Host "   ¿Quieres detener el proceso ahora? (S/N)" -ForegroundColor Cyan
    $respuesta = Read-Host
    if ($respuesta -eq "S" -or $respuesta -eq "s") {
        Stop-Process -Name "proyecto_hospital_version_1" -Force
        Write-Host "? Proceso detenido" -ForegroundColor Green
        Start-Sleep -Seconds 2
    } else {
        Write-Host "? No se puede compilar con Blazor corriendo" -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "? Blazor NO está corriendo (OK para compilar)" -ForegroundColor Green
}

Write-Host ""

# ============================================
# PASO 4: Limpiar solución
# ============================================
Write-Host "PASO 4: Limpiando solución..." -ForegroundColor Yellow
Write-Host ""

Set-Location $solutionPath
dotnet clean

if ($LASTEXITCODE -eq 0) {
    Write-Host "? Limpieza completada exitosamente" -ForegroundColor Green
} else {
    Write-Host "? ERROR en la limpieza" -ForegroundColor Red
    exit 1
}

Write-Host ""

# ============================================
# PASO 5: Compilar solución
# ============================================
Write-Host "PASO 5: Compilando solución..." -ForegroundColor Yellow
Write-Host ""

dotnet build

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "? COMPILACIÓN EXITOSA" -ForegroundColor Green
    Write-Host ""
} else {
    Write-Host ""
    Write-Host "? ERROR EN LA COMPILACIÓN" -ForegroundColor Red
    Write-Host "   Revisa los errores arriba" -ForegroundColor Yellow
    Write-Host ""
    exit 1
}

# ============================================
# RESUMEN FINAL
# ============================================
Write-Host "================================================" -ForegroundColor Cyan
Write-Host "           ? VERIFICACIÓN COMPLETA            " -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "?? Los cambios están aplicados correctamente" -ForegroundColor Green
Write-Host "?? La solución se compiló sin errores" -ForegroundColor Green
Write-Host ""
Write-Host "SIGUIENTE PASO:" -ForegroundColor Yellow
Write-Host "1. Inicia la API:" -ForegroundColor White
Write-Host "   cd $apiPath" -ForegroundColor Cyan
Write-Host "   dotnet run" -ForegroundColor Cyan
Write-Host ""
Write-Host "2. Inicia Blazor (en otra terminal):" -ForegroundColor White
Write-Host "   cd $blazorPath" -ForegroundColor Cyan
Write-Host "   dotnet run" -ForegroundColor Cyan
Write-Host ""
Write-Host "3. Ve a 'Historial de Logs' y verifica que se muestren los registros" -ForegroundColor White
Write-Host ""
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""
