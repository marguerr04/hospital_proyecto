# ?? GUÍA DE EJECUCIÓN: Solución Error Historial de Logs

## ?? IMPORTANTE: SIGUE ESTOS PASOS EN ORDEN

---

## ?? **PASO 1: Detén todas las aplicaciones**

### Detén la API (Hospital.Api):
- Si está corriendo en Visual Studio ? Presiona el botón **DETENER** (??) o `Shift + F5`
- Si está corriendo en terminal ? Presiona `Ctrl + C`

### Detén el proyecto Blazor (proyecto_hospital_version_1):
- Si está corriendo en Visual Studio ? Presiona el botón **DETENER** (??) o `Shift + F5`
- Si está corriendo en terminal ? Presiona `Ctrl + C`

**? Verificar:** Ningún proceso debe estar usando los archivos `.exe` de los proyectos.

---

## ?? **PASO 2: Limpia y recompila la solución**

### Opción A: Desde Visual Studio

1. Click derecho en la **Solución** (en el Solution Explorer)
2. Click en **"Clean Solution"** (Limpiar solución)
3. Espera a que termine
4. Click derecho en la **Solución** nuevamente
5. Click en **"Rebuild Solution"** (Recompilar solución)
6. Verifica que no haya errores de compilación

### Opción B: Desde PowerShell/Terminal

```powershell
# Navega al directorio de la solución
cd E:\Retorno_cero\Testeo_recuperacion_1\HospitalSolution

# Limpia la solución completa
dotnet clean

# Recompila la solución completa
dotnet build
```

**? Verificar:** El build debe completarse sin errores.

---

## ?? **PASO 3: Inicia las aplicaciones en el orden correcto**

### 3.1 Primero: Inicia la API

```powershell
cd E:\Retorno_cero\Testeo_recuperacion_1\Hospital.Api
dotnet run
```

**? Verificar:** Debes ver el mensaje:
```
Now listening on: https://localhost:7032
Now listening on: http://localhost:5032
```

### 3.2 Segundo: Inicia la aplicación Blazor

**Desde otra terminal:**
```powershell
cd E:\Retorno_cero\Testeo_recuperacion_1\proyecto_hospital_version_1
dotnet run
```

**? Verificar:** Debes ver el mensaje con la URL de la aplicación.

---

## ?? **PASO 4: Prueba el Historial de Logs**

1. **Abre el navegador** en la URL de la aplicación Blazor
2. **Inicia sesión** con tus credenciales
3. **Navega** a la sección **"Historial de Logs - Auditoría de Priorizaciones"**
4. **Verifica** que la tabla ahora muestre los registros correctamente:

### ? Lo que debes ver:

```
?? Total: 21 registros

?????????????????????????????????????????????????????????????????????????????????????????
? AudId   ? Fecha              ? Acción   ? Usuario             ? Solicitud  ? Prioridad?
?????????????????????????????????????????????????????????????????????????????????????????
? 21      ? 22/01/2025 11:30   ? INSERT   ? marguerr04          ? 123        ? P1       ?
? 20      ? 22/01/2025 11:25   ? UPDATE   ? admin               ? 122        ? P2       ?
? ...     ? ...                ? ...      ? ...                 ? ...        ? ...      ?
?????????????????????????????????????????????????????????????????????????????????????????
```

### ? Si sigues viendo el error:

1. Verifica que los cambios se hayan guardado:
   ```powershell
   # Verifica el archivo DTO
   Get-Content "E:\Retorno_cero\Testeo_recuperacion_1\Hospital.Api\DTOs\AuditoriaPriorizacionDto.cs" | Select-String "public byte Prioridad"
   
   # Debe retornar:
   # public byte Prioridad { get; set; }  // Cambiado de int a byte...
   ```

2. Revisa los logs de la API en la terminal para ver si hay otros errores

3. Limpia completamente y recompila de nuevo (PASO 2)

---

## ?? **PASO 5: Verificación Final**

### Checklist de Verificación:

- [ ] ? La API está corriendo sin errores
- [ ] ? La aplicación Blazor está corriendo sin errores  
- [ ] ? Puedes iniciar sesión correctamente
- [ ] ? La página "Historial de Logs" carga sin errores
- [ ] ? Se muestra "Total: 21 registros" (o el número que corresponda)
- [ ] ? La tabla muestra los registros de auditoría con todos los campos
- [ ] ? Los badges de prioridad (P1, P2, P3) se muestran con colores correctos:
  - P1 ? Rojo (bg-danger)
  - P2 ? Amarillo (bg-warning)
  - P3 ? Azul (bg-primary)

---

## ??? **Troubleshooting (Resolución de Problemas)**

### Problema 1: "The process cannot access the file because it is being used by another process"

**Solución:**
```powershell
# Encuentra el proceso que está usando el puerto
netstat -ano | findstr :7032

# Mata el proceso (reemplaza PID con el número que encontraste)
taskkill /PID [PID] /F
```

### Problema 2: "No se puede encontrar el archivo AuditoriaPriorizacionDto.cs"

**Solución:**
Verifica la ruta completa:
```powershell
Test-Path "E:\Retorno_cero\Testeo_recuperacion_1\Hospital.Api\DTOs\AuditoriaPriorizacionDto.cs"
```

### Problema 3: Sigue apareciendo "No hay registros de auditoría"

**Solución:**
1. Verifica la conexión a la base de datos en `appsettings.json`
2. Ejecuta el script de verificación SQL:
   ```sql
   SELECT COUNT(*) FROM [dbo].[AUD_PRIORIZACION_SOLICITUD];
   ```
3. Revisa los logs de la API para ver el error específico

---

## ?? **Documentación Relacionada**

- `SOLUCION_ERROR_INVALIDCAST_PRIORIDAD.md` - Explicación técnica detallada
- `SQL_VERIFICACION_COMPLETA.sql` - Script para verificar los datos en BD

---

## ? **Estado Final Esperado**

Después de completar esta guía, deberías tener:

1. ? API corriendo sin errores de compilación
2. ? Blazor corriendo sin errores de compilación
3. ? Página de Historial de Logs mostrando datos correctamente
4. ? Sin errores `InvalidCastException` en los logs
5. ? Tabla de auditoría completamente funcional con paginación

---

## ?? Fecha
2025-01-22

## ?? Soporte
Si después de seguir esta guía sigues teniendo problemas, proporciona:
- Logs completos de la API
- Logs completos de la aplicación Blazor
- Captura de pantalla del error
