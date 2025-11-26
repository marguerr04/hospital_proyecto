# Script DEFINITIVO para eliminar el segundo buscador duplicado
# Este script elimina TODO entre la línea 289 y la línea que contiene "@if (_mostrarInfoPaciente"

$filePath = "..\proyecto_hospital_version_1\Components\Pages\Dashboard.razor"

Write-Host "============================================" -ForegroundColor Cyan
Write-Host "ELIMINACIÓN DEFINITIVA DEL BUSCADOR DUPLICADO" -ForegroundColor Cyan
Write-Host "============================================`n" -ForegroundColor Cyan

# Leer todo el archivo
$content = Get-Content $filePath -Raw

Write-Host "Buscando patrones..." -ForegroundColor Yellow

# Patrón más agresivo: desde el cierre de Profesionales hasta el primer modal
$patron = '(?s)(</div>\s*</div>\s*</div>\s*</div>\s*<div class="row".*?)</div>\s*</div>\s*@if \(_mostrarInfoPaciente'

if ($content -match $patron) {
    Write-Host "? Patrón encontrado, eliminando sección duplicada..." -ForegroundColor Green
    
    # Reemplazar el patrón manteniendo solo los cierres necesarios
    $content = $content -replace $patron, '$1@if (_mostrarInfoPaciente'
    
    # Guardar el archivo
    Set-Content $filePath -Value $content -Encoding UTF8 -NoNewline
    
    Write-Host "? Sección eliminada exitosamente!" -ForegroundColor Green
} else {
    Write-Host "?? No se encontró el patrón exacto, intentando método alternativo..." -ForegroundColor Yellow
    
    # Método alternativo: eliminar por conteo de líneas
    $lines = Get-Content $filePath
    Write-Host "Total de líneas: $($lines.Count)" -ForegroundColor White
    
    # Buscar línea que contiene "Profesionales Médicos" (última)
    $lineaProfesionales = -1
    for ($i = $lines.Count - 1; $i -ge 0; $i--) {
        if ($lines[$i] -match "Profesionales Médicos") {
            $lineaProfesionales = $i
            break
        }
    }
    
    # Buscar línea que contiene "@if (_mostrarInfoPaciente"
    $lineaModal = -1
    for ($i = 0; $i -lt $lines.Count; $i++) {
        if ($lines[$i] -match "@if \(_mostrarInfoPaciente") {
            $lineaModal = $i
            break
        }
    }
    
    if ($lineaProfesionales -gt 0 -and $lineaModal -gt 0) {
        Write-Host "Línea Profesionales: $lineaProfesionales" -ForegroundColor White
        Write-Host "Línea Modal: $lineaModal" -ForegroundColor White
        
        # Calcular cuántas líneas hay entre Profesionales y Modal
        $lineasEntreMedio = $lineaModal - $lineaProfesionales - 10
        
        if ($lineasEntreMedio > 50) {
            Write-Host "Se encontraron $lineasEntreMedio líneas sospechosas entre Profesionales y Modal" -ForegroundColor Yellow
            
            # Reconstruir el archivo sin las líneas del medio
            $nuevasLineas = @()
            $nuevasLineas += $lines[0..($lineaProfesionales + 9)]  # Incluir hasta 10 líneas después de Profesionales
            $nuevasLineas += "    </div>"  # Cierre final
            $nuevasLineas += ""
            $nuevasLineas += $lines[$lineaModal..($lines.Count - 1)]  # Desde el modal hasta el final
            
            Set-Content $filePath -Value $nuevasLineas -Encoding UTF8
            Write-Host "? Archivo reconstruido sin el buscador duplicado" -ForegroundColor Green
        } else {
            Write-Host "? No se detectaron suficientes líneas duplicadas" -ForegroundColor Red
        }
    } else {
        Write-Host "? No se pudieron encontrar los marcadores necesarios" -ForegroundColor Red
    }
}

Write-Host "`n============================================" -ForegroundColor Cyan
Write-Host "VERIFICACIÓN FINAL" -ForegroundColor Cyan
Write-Host "============================================`n" -ForegroundColor Cyan

# Contar cuántos "Buscar Paciente" quedan
$verificacion = Select-String -Path $filePath -Pattern "Buscar Paciente"
Write-Host "Total de 'Buscar Paciente' encontrados: $($verificacion.Count)" -ForegroundColor White

$verificacion | ForEach-Object {
    Write-Host "  Línea $($_.LineNumber): $($_.Line.Trim())" -ForegroundColor Gray
}

if ($verificacion.Count -eq 1) {
    Write-Host "`n? ¡ÉXITO! Solo queda 1 buscador" -ForegroundColor Green
} else {
    Write-Host "`n?? Todavía hay $($verificacion.Count) buscadores" -ForegroundColor Red
    Write-Host "Ejecuta el script nuevamente o edita manualmente" -ForegroundColor Yellow
}

Write-Host "`n============================================" -ForegroundColor Cyan
Write-Host "Proceso completado" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
