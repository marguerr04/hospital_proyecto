# Script para eliminar el segundo buscador duplicado en Dashboard.razor
# Elimina líneas 291-406

$filePath = "..\proyecto_hospital_version_1\Components\Pages\Dashboard.razor"
$content = Get-Content $filePath

Write-Host "Total de líneas actuales: $($content.Length)" -ForegroundColor Cyan
Write-Host "Eliminando líneas 291-406..." -ForegroundColor Yellow

# Mantener líneas 1-290 y 407-final
$lineasAntesDeEliminar = $content.Length
$newContent = $content[0..289] + $content[406..($content.Length-1)]

# Guardar el archivo
Set-Content $filePath -Value $newContent -Encoding UTF8

Write-Host "? Eliminación completada!" -ForegroundColor Green
Write-Host "Líneas antes: $lineasAntesDeEliminar" -ForegroundColor White
Write-Host "Líneas después: $($newContent.Length)" -ForegroundColor White
Write-Host "Líneas eliminadas: $($lineasAntesDeEliminar - $newContent.Length)" -ForegroundColor White

# Verificar que solo quede 1 "Buscar Paciente"
$verificacion = Select-String -Path $filePath -Pattern "Buscar Paciente"
Write-Host "`nVerificación - 'Buscar Paciente' encontrado:" -ForegroundColor Cyan
$verificacion | ForEach-Object { Write-Host "  Línea $($_.LineNumber): $($_.Line.Trim())" }

if ($verificacion.Count -eq 1) {
    Write-Host "`n? ¡ÉXITO! Solo queda 1 buscador" -ForegroundColor Green
} else {
    Write-Host "`n?? ADVERTENCIA: Aún hay $($verificacion.Count) buscadores" -ForegroundColor Yellow
}
