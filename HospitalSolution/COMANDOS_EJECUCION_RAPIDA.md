# ? COMANDOS DE EJECUCIÓN RÁPIDA

## ?? PASO 1: POBLAR BASE DE DATOS

### Opción A: SQL Server Management Studio (Recomendado)
```sql
-- 1. Abrir SQL Server Management Studio
-- 2. Conectarse a la instancia de SQL Server
-- 3. Abrir el archivo: SQL_POBLAR_PROFESIONALES.sql
-- 4. Presionar F5 o hacer click en "Execute"
-- 5. Verificar mensaje: "? Se insertaron 20 profesionales médicos exitosamente"
```

### Opción B: sqlcmd (Línea de comandos)
```bash
sqlcmd -S . -d HospitalV4 -i SQL_POBLAR_PROFESIONALES.sql
```

---

## ?? PASO 2: INICIAR API (Terminal 1)

### Windows (PowerShell)
```powershell
cd E:\Retorno_cero\Testeo_recuperacion_1\Hospital.Api
dotnet run
```

### Windows (CMD)
```cmd
cd E:\Retorno_cero\Testeo_recuperacion_1\Hospital.Api
dotnet run
```

### Linux/macOS
```bash
cd ~/Retorno_cero/Testeo_recuperacion_1/Hospital.Api
dotnet run
```

**Resultado esperado**:
```
Now listening on: https://localhost:7032
Application started. Press Ctrl+C to shut down.
```

---

## ?? PASO 3: INICIAR BLAZOR (Terminal 2)

### Windows (PowerShell)
```powershell
cd E:\Retorno_cero\Testeo_recuperacion_1\proyecto_hospital_version_1
dotnet run
```

### Windows (CMD)
```cmd
cd E:\Retorno_cero\Testeo_recuperacion_1\proyecto_hospital_version_1
dotnet run
```

### Linux/macOS
```bash
cd ~/Retorno_cero/Testeo_recuperacion_1/proyecto_hospital_version_1
dotnet run
```

**Resultado esperado**:
```
Now listening on: https://localhost:7213
Application started. Press Ctrl+C to shut down.
```

---

## ?? PASO 4: ABRIR NAVEGADOR

### Windows (PowerShell)
```powershell
Start-Process "https://localhost:7213/dashboard"
```

### Windows (CMD)
```cmd
start https://localhost:7213/dashboard
```

### Linux
```bash
xdg-open https://localhost:7213/dashboard
```

### macOS
```bash
open https://localhost:7213/dashboard
```

### Manual
```
1. Abrir navegador (Chrome, Edge, Firefox)
2. Navegar a: https://localhost:7213/dashboard
3. Click en el botón verde: "Historial de Logs"
```

---

## ? VERIFICACIÓN RÁPIDA

### Verificar API (PowerShell)
```powershell
Invoke-WebRequest -Uri "https://localhost:7032/api/AuditoriaPriorizacion" -SkipCertificateCheck | ConvertFrom-Json
```

### Verificar API (curl)
```bash
curl -k https://localhost:7032/api/AuditoriaPriorizacion
```

### Verificar Swagger
```
https://localhost:7032/swagger
```

### Verificar Base de Datos (SQL)
```sql
-- Verificar profesionales
SELECT COUNT(*) AS TotalProfesionales FROM [dbo].[PROFESIONAL];

-- Verificar auditoría
SELECT COUNT(*) AS TotalAuditoria FROM [dbo].[AUD_PRIORIZACION_SOLICITUD];

-- Ver últimos registros
SELECT TOP 5 * FROM [dbo].[PROFESIONAL] ORDER BY Id DESC;
SELECT TOP 5 * FROM [dbo].[AUD_PRIORIZACION_SOLICITUD] ORDER BY AudFecha DESC;
```

---

## ?? COMANDOS DE DIAGNÓSTICO

### Ver puertos en uso (Windows PowerShell)
```powershell
Get-NetTCPConnection -LocalPort 7032,7213 | Select-Object LocalPort, State, OwningProcess
```

### Ver puertos en uso (Linux/macOS)
```bash
netstat -tuln | grep -E '7032|7213'
```

### Ver procesos de .NET (PowerShell)
```powershell
Get-Process dotnet
```

### Ver procesos de .NET (Linux/macOS)
```bash
ps aux | grep dotnet
```

### Matar procesos de .NET (Windows PowerShell)
```powershell
Get-Process dotnet | Stop-Process -Force
```

### Matar procesos de .NET (Linux/macOS)
```bash
killall dotnet
```

---

## ?? LIMPIAR Y RECOMPILAR

### Limpiar y recompilar API
```bash
cd Hospital.Api
dotnet clean
dotnet build
dotnet run
```

### Limpiar y recompilar Blazor
```bash
cd proyecto_hospital_version_1
dotnet clean
dotnet build
dotnet run
```

### Limpiar todo (PowerShell)
```powershell
cd Hospital.Api
dotnet clean
cd ..\proyecto_hospital_version_1
dotnet clean
cd ..
```

---

## ?? REINICIAR TODO (Script Completo)

### Windows (PowerShell)
```powershell
# Matar procesos existentes
Get-Process dotnet -ErrorAction SilentlyContinue | Stop-Process -Force

# Esperar 2 segundos
Start-Sleep -Seconds 2

# Iniciar API en nueva ventana
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd E:\Retorno_cero\Testeo_recuperacion_1\Hospital.Api; dotnet run"

# Esperar 5 segundos
Start-Sleep -Seconds 5

# Iniciar Blazor en nueva ventana
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd E:\Retorno_cero\Testeo_recuperacion_1\proyecto_hospital_version_1; dotnet run"

# Esperar 5 segundos
Start-Sleep -Seconds 5

# Abrir navegador
Start-Process "https://localhost:7213/dashboard"
```

### Linux/macOS (Bash)
```bash
#!/bin/bash

# Matar procesos existentes
killall dotnet 2>/dev/null

# Esperar 2 segundos
sleep 2

# Iniciar API en background
cd ~/Retorno_cero/Testeo_recuperacion_1/Hospital.Api
dotnet run &
API_PID=$!

# Esperar 5 segundos
sleep 5

# Iniciar Blazor en background
cd ~/Retorno_cero/Testeo_recuperacion_1/proyecto_hospital_version_1
dotnet run &
BLAZOR_PID=$!

# Esperar 5 segundos
sleep 5

# Abrir navegador
xdg-open https://localhost:7213/dashboard 2>/dev/null || open https://localhost:7213/dashboard

echo "API PID: $API_PID"
echo "Blazor PID: $BLAZOR_PID"
```

---

## ?? TROUBLESHOOTING RÁPIDO

### Error: "Puerto 7032 ya está en uso"
```powershell
# Windows
Get-NetTCPConnection -LocalPort 7032 | ForEach-Object { Stop-Process -Id $_.OwningProcess -Force }

# Linux/macOS
lsof -ti:7032 | xargs kill -9
```

### Error: "Puerto 7213 ya está en uso"
```powershell
# Windows
Get-NetTCPConnection -LocalPort 7213 | ForEach-Object { Stop-Process -Id $_.OwningProcess -Force }

# Linux/macOS
lsof -ti:7213 | xargs kill -9
```

### Error: "No se puede conectar a la base de datos"
```sql
-- Verificar conexión
SELECT @@VERSION;
SELECT DB_NAME();
```

---

## ?? SCRIPT DE INICIO AUTOMÁTICO (PowerShell)

Guarda este script como `INICIAR_PROYECTO.ps1`:

```powershell
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "?? INICIANDO PROYECTO HOSPITAL" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 1. Matar procesos existentes
Write-Host "?? Limpiando procesos anteriores..." -ForegroundColor Yellow
Get-Process dotnet -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep -Seconds 2

# 2. Iniciar API
Write-Host "?? Iniciando API..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd E:\Retorno_cero\Testeo_recuperacion_1\Hospital.Api; dotnet run"
Start-Sleep -Seconds 5

# 3. Iniciar Blazor
Write-Host "?? Iniciando Blazor..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd E:\Retorno_cero\Testeo_recuperacion_1\proyecto_hospital_version_1; dotnet run"
Start-Sleep -Seconds 5

# 4. Abrir navegador
Write-Host "?? Abriendo navegador..." -ForegroundColor Yellow
Start-Process "https://localhost:7213/dashboard"

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "? PROYECTO INICIADO EXITOSAMENTE" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "?? API: https://localhost:7032" -ForegroundColor White
Write-Host "?? Blazor: https://localhost:7213" -ForegroundColor White
Write-Host "?? Dashboard: https://localhost:7213/dashboard" -ForegroundColor White
Write-Host ""

Read-Host "Presiona Enter para cerrar"
```

**Ejecutar**:
```powershell
.\INICIAR_PROYECTO.ps1
```

---

## ? COMANDOS COMPLETOS EN ORDEN

```powershell
# 1. Poblar Base de Datos
# ? Ejecutar SQL_POBLAR_PROFESIONALES.sql en SSMS

# 2. Iniciar API
cd E:\Retorno_cero\Testeo_recuperacion_1\Hospital.Api
dotnet run

# 3. Iniciar Blazor (nueva terminal)
cd E:\Retorno_cero\Testeo_recuperacion_1\proyecto_hospital_version_1
dotnet run

# 4. Abrir navegador
Start-Process "https://localhost:7213/dashboard"

# 5. Click en "Historial de Logs"
```

---

## ?? ¡LISTO!

**Sigue estos comandos en orden y todo funcionará perfectamente.**
