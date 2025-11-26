# ?? COMANDOS DE VERIFICACIÓN RÁPIDA

## ? Verificar que los cambios están aplicados

```powershell
# Verificar Archivo 1: AuditoriaPriorizacionDto.cs
Get-Content "E:\Retorno_cero\Testeo_recuperacion_1\Hospital.Api\DTOs\AuditoriaPriorizacionDto.cs" | Select-String "byte Prioridad"

# Debe mostrar:
# public byte Prioridad { get; set; }  // Cambiado de int a byte para coincidir con tinyint de BD

# ????????????????????????????????????????????????????????????

# Verificar Archivo 2: AuditoriaPriorizacionService.cs
Get-Content "E:\Retorno_cero\Testeo_recuperacion_1\Hospital.Api\Data\Services\AuditoriaPriorizacionService.cs" | Select-String "byte Prioridad"

# Debe mostrar:
# public byte Prioridad { get; set; }  // Cambiado de int a byte para coincidir con tinyint de BD
```

---

## ?? Verificar que no hay procesos corriendo

```powershell
# Verificar si la API está corriendo
Get-Process -Name "Hospital.Api" -ErrorAction SilentlyContinue

# Si muestra algo ? La API está corriendo (debes detenerla)
# Si no muestra nada ? OK para compilar

# ????????????????????????????????????????????????????????????

# Verificar si Blazor está corriendo
Get-Process -Name "proyecto_hospital_version_1" -ErrorAction SilentlyContinue

# Si muestra algo ? Blazor está corriendo (debes detenerla)
# Si no muestra nada ? OK para compilar

# ????????????????????????????????????????????????????????????

# Verificar qué está usando el puerto de la API
netstat -ano | findstr :7032

# Si muestra algo ? Hay un proceso usando el puerto
# Anota el PID (última columna) y mátalo:
# taskkill /PID [número] /F
```

---

## ?? Limpiar y Compilar

```powershell
# Navega a la solución
cd E:\Retorno_cero\Testeo_recuperacion_1\HospitalSolution

# Limpia la solución
dotnet clean

# Compila la solución
dotnet build

# Si ves "Build succeeded" ? ? TODO OK
# Si ves errores ? ? Revisa los mensajes de error
```

---

## ?? Iniciar las aplicaciones

```powershell
# ????????????????????????????????????????????????????????????
# TERMINAL 1: Inicia la API
# ????????????????????????????????????????????????????????????

cd E:\Retorno_cero\Testeo_recuperacion_1\Hospital.Api
dotnet run

# Debes ver:
# Now listening on: https://localhost:7032
# Now listening on: http://localhost:5032

# ????????????????????????????????????????????????????????????
# TERMINAL 2: Inicia Blazor (en OTRA terminal)
# ????????????????????????????????????????????????????????????

cd E:\Retorno_cero\Testeo_recuperacion_1\proyecto_hospital_version_1
dotnet run

# Debes ver la URL de la aplicación Blazor
```

---

## ?? Verificar los Logs

```powershell
# En la terminal de la API, busca esta línea:
# info: Microsoft.EntityFrameworkCore.Database.Command[20101]
#       Executed DbCommand (...) [...]
#       SELECT [AudId], [AudFecha], ... FROM [dbo].[AUD_PRIORIZACION_SOLICITUD]

# ? Si ves esto SIN error InvalidCastException ? TODO OK
# ? Si ves "System.InvalidCastException" ? Algo salió mal
```

---

## ??? Verificar la Base de Datos

```sql
-- Conecta a SQL Server y ejecuta:

USE HospitalV4;
GO

-- Contar total de registros
SELECT COUNT(*) AS TotalRegistros 
FROM [dbo].[AUD_PRIORIZACION_SOLICITUD];
GO

-- Debe retornar: 21 (o el número correcto)

-- Ver tipo de columna prioridad
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'AUD_PRIORIZACION_SOLICITUD'
  AND COLUMN_NAME = 'prioridad';
GO

-- Debe retornar:
-- COLUMN_NAME: prioridad
-- DATA_TYPE: tinyint
-- CHARACTER_MAXIMUM_LENGTH: NULL

-- Ver algunos registros
SELECT TOP 5
    AudId,
    AudFecha,
    AudAccion,
    AudUsuario,
    prioridad
FROM [dbo].[AUD_PRIORIZACION_SOLICITUD]
ORDER BY AudFecha DESC;
GO
```

---

## ?? Probar en el Navegador

```
1. Abre el navegador
2. Ve a la URL de Blazor (ejemplo: https://localhost:5001)
3. Inicia sesión
4. Navega a "Historial de Logs - Auditoría de Priorizaciones"
5. Verifica:
   ? Se muestra "Total: 21 registros"
   ? La tabla tiene datos
   ? Los badges de prioridad tienen colores:
      • P1 ? Rojo (bg-danger)
      • P2 ? Amarillo (bg-warning)
      • P3 ? Azul (bg-primary)
```

---

## ?? Comandos de Emergencia

```powershell
# ????????????????????????????????????????????????????????????
# Si algo va mal, usa estos comandos:
# ????????????????????????????????????????????????????????????

# Matar todos los procesos de dotnet
Get-Process -Name "dotnet" | Stop-Process -Force

# Matar proceso específico por PID
taskkill /PID [número] /F

# Limpiar completamente los binarios
cd E:\Retorno_cero\Testeo_recuperacion_1\Hospital.Api
Remove-Item -Recurse -Force bin, obj

cd E:\Retorno_cero\Testeo_recuperacion_1\proyecto_hospital_version_1
Remove-Item -Recurse -Force bin, obj

# Recompilar desde cero
cd E:\Retorno_cero\Testeo_recuperacion_1\HospitalSolution
dotnet clean
dotnet restore
dotnet build

# Verificar versión de .NET
dotnet --version

# Debe mostrar: 8.x.x (o superior)

# Verificar SDKs instalados
dotnet --list-sdks

# Debe incluir: 8.0.x
```

---

## ?? Checklist de Verificación

Marca cada item cuando lo completes:

- [ ] ? Los archivos modificados tienen `byte Prioridad`
- [ ] ? No hay procesos de API o Blazor corriendo
- [ ] ? `dotnet clean` se ejecutó sin errores
- [ ] ? `dotnet build` se ejecutó sin errores
- [ ] ? La API inició correctamente (puerto 7032)
- [ ] ? Blazor inició correctamente
- [ ] ? Puedo acceder a la aplicación en el navegador
- [ ] ? La página "Historial de Logs" carga
- [ ] ? Se muestra "Total: 21 registros"
- [ ] ? La tabla muestra los datos
- [ ] ? Los badges de prioridad tienen colores correctos
- [ ] ? No hay errores `InvalidCastException` en los logs

---

## ?? Comando Todo-en-Uno (Script Automatizado)

Si prefieres que un script haga todo por ti:

```powershell
cd E:\Retorno_cero\Testeo_recuperacion_1\HospitalSolution
.\VERIFICAR_Y_COMPILAR.ps1
```

El script hará:
1. ? Verificar archivos
2. ? Confirmar cambios
3. ? Detener procesos (con confirmación)
4. ? Limpiar solución
5. ? Compilar solución
6. ? Mostrar siguiente pasos

---

## ?? Copiar y Pegar

### Secuencia Completa (Copia y pega todo):

```powershell
# 1. Verificar cambios
Get-Content "E:\Retorno_cero\Testeo_recuperacion_1\Hospital.Api\DTOs\AuditoriaPriorizacionDto.cs" | Select-String "byte Prioridad"

# 2. Navegar a la solución
cd E:\Retorno_cero\Testeo_recuperacion_1\HospitalSolution

# 3. Limpiar y compilar
dotnet clean
dotnet build

# 4. Ir a la API
cd ..\Hospital.Api

# 5. Iniciar API (en esta terminal)
dotnet run

# 6. En OTRA terminal, ir a Blazor
cd E:\Retorno_cero\Testeo_recuperacion_1\proyecto_hospital_version_1

# 7. Iniciar Blazor
dotnet run

# 8. Abrir navegador y probar
```

---

## ?? Si Nada Funciona

```powershell
# Último recurso: Rebootear Visual Studio y terminal
# 1. Cierra Visual Studio
# 2. Cierra todas las terminales
# 3. Abre Visual Studio de nuevo
# 4. Abre una terminal nueva
# 5. Ejecuta el script automatizado:
cd E:\Retorno_cero\Testeo_recuperacion_1\HospitalSolution
.\VERIFICAR_Y_COMPILAR.ps1
```

---

**Última actualización:** 2025-01-22  
**Estado:** ? COMANDOS VERIFICADOS
