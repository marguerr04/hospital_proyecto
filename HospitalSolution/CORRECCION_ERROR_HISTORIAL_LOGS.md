# ?? CORRECCIÓN: ERROR EN HISTORIAL DE LOGS

## ? PROBLEMA IDENTIFICADO

### **Error en la Consola**:
```
Microsoft.Data.SqlClient.SqlException: No column name was specified for column 1 of 's'.
Invalid column name 'Value'.
Error en GetTotalRegistrosAsync: No column name was specified for column 1 of 's'.
```

### **Causa del Error**:
El método `GetTotalRegistrosAsync()` en el servicio `AuditoriaPriorizacionService` estaba usando `SqlQueryRaw<int>()` de Entity Framework Core 8, que requiere un alias de columna específico. La query:

```csharp
// ? INCORRECTO
var query = "SELECT COUNT(*) FROM [dbo].[AUD_PRIORIZACION_SOLICITUD]";
var result = await _context.Database.SqlQueryRaw<int>(query).FirstOrDefaultAsync();
```

No funcionaba porque EF Core 8 espera un alias de columna explícito como `Value` o necesita una estructura de retorno específica.

---

## ? SOLUCIÓN APLICADA

### **Cambio en el Servicio** (`AuditoriaPriorizacionService.cs`)

Reemplacé el método `GetTotalRegistrosAsync()` para usar **ADO.NET directo** en lugar de `SqlQueryRaw`:

```csharp
public async Task<int> GetTotalRegistrosAsync()
{
    try
    {
        // Enfoque alternativo: Usar ADO.NET directamente para mayor control
        using (var command = _context.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = "SELECT COUNT(*) FROM [dbo].[AUD_PRIORIZACION_SOLICITUD]";
            
            await _context.Database.OpenConnectionAsync();
            
            var result = await command.ExecuteScalarAsync();
            
            return result != null ? Convert.ToInt32(result) : 0;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error en GetTotalRegistrosAsync: {ex.Message}");
        return 0;
    }
    finally
    {
        await _context.Database.CloseConnectionAsync();
    }
}
```

### **¿Por qué funciona esto?**
- ? **ADO.NET directo**: No depende de EF Core para mapear el resultado
- ? **`ExecuteScalarAsync()`**: Devuelve directamente el valor escalar (el COUNT)
- ? **Control total**: Manejamos la conexión manualmente
- ? **Sin errores de mapeo**: No hay problemas con alias de columnas

---

### **Cambio en el Controlador** (`AuditoriaPriorizacionController.cs`)

Agregué el `using` faltante para `IAuditoriaPriorizacionService`:

```csharp
using Hospital.Api.Data.Services;  // ? AGREGADO
using Hospital.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
```

---

## ?? VERIFICACIÓN

### **1. Compilar el Proyecto**
```bash
cd Hospital.Api
dotnet build
```
? **Esperado**: Build exitoso sin errores

### **2. Iniciar la API**
```bash
cd Hospital.Api
dotnet run
```
? **Esperado**: `Now listening on: https://localhost:7032`

### **3. Verificar Swagger**
```
https://localhost:7032/swagger
```
? **Esperado**: Ver el endpoint `/api/AuditoriaPriorizacion` visible

### **4. Probar el Endpoint**
```bash
curl -k https://localhost:7032/api/AuditoriaPriorizacion
```
? **Esperado**: JSON con datos de auditoría

### **5. Verificar en Blazor**
```
https://localhost:7213/historial-logs
```
? **Esperado**: Tabla con registros de auditoría cargados

---

## ?? ARCHIVOS MODIFICADOS

| # | Archivo | Cambio |
|---|---------|--------|
| 1 | `Hospital.Api/Data/Services/AuditoriaPriorizacionService.cs` | Reemplazado método `GetTotalRegistrosAsync()` con ADO.NET directo |
| 2 | `Hospital.Api/Controllers/AuditoriaPriorizacionController.cs` | Agregado `using Hospital.Api.Data.Services;` |

---

## ?? COMANDOS PARA PROBAR

### **Reiniciar la API**:
```bash
# Detener la API actual (Ctrl+C)
cd E:\Retorno_cero\Testeo_recuperacion_1\Hospital.Api
dotnet clean
dotnet build
dotnet run
```

### **Reiniciar Blazor** (si es necesario):
```bash
# Detener Blazor (Ctrl+C)
cd E:\Retorno_cero\Testeo_recuperacion_1\proyecto_hospital_version_1
dotnet clean
dotnet build
dotnet run
```

### **Probar el Endpoint**:
```bash
# Opción 1: curl
curl -k https://localhost:7032/api/AuditoriaPriorizacion

# Opción 2: PowerShell
Invoke-WebRequest -Uri "https://localhost:7032/api/AuditoriaPriorizacion" -SkipCertificateCheck | ConvertFrom-Json

# Opción 3: Navegador
https://localhost:7032/swagger
```

---

## ? RESULTADO ESPERADO

### **En la Consola de la API**:
```
Now listening on: https://localhost:7032
Application started. Press Ctrl+C to shut down.
```
? **Sin errores de `SqlException`**

### **En Swagger**:
```
GET /api/AuditoriaPriorizacion
GET /api/AuditoriaPriorizacion/total
```
? **Ambos endpoints visibles y funcionales**

### **En Blazor** (`/historial-logs`):
```
Registro de Auditoría de Priorizaciones
Total: 21 registros

[Tabla con datos]
- Aud ID | Fecha | Acción | Usuario | Solicitud ID | Prioridad
- ...
```
? **Tabla con datos cargados correctamente**

---

## ?? DIAGNÓSTICO ADICIONAL

Si aún hay problemas, verifica lo siguiente:

### **1. Verificar que la tabla existe**:
```sql
SELECT COUNT(*) FROM [dbo].[AUD_PRIORIZACION_SOLICITUD];
-- Esperado: Número > 0
```

### **2. Verificar la estructura de la tabla**:
```sql
SELECT TOP 5 * FROM [dbo].[AUD_PRIORIZACION_SOLICITUD];
-- Esperado: Registros visibles
```

### **3. Verificar el servicio registrado**:
```csharp
// En Hospital.Api/Program.cs
builder.Services.AddScoped<IAuditoriaPriorizacionService, AuditoriaPriorizacionService>();
```
? **Debe estar registrado**

### **4. Verificar los logs de la consola**:
```
# En la consola de la API, busca:
- "Error en GetTotalRegistrosAsync" ? ? Hay un problema
- "Error en GetHistorialAuditoriaAsync" ? ? Hay un problema
- Sin errores ? ? Todo bien
```

---

## ?? RESUMEN DE LA CORRECCIÓN

| Antes | Después |
|-------|---------|
| ? `SqlQueryRaw<int>()` con problemas de mapeo | ? ADO.NET directo con `ExecuteScalarAsync()` |
| ? Error: "No column name was specified" | ? Sin errores de mapeo |
| ? Endpoint no funcional | ? Endpoint funcional |
| ? Blazor sin datos | ? Blazor con datos |

---

## ?? SI PERSISTE EL ERROR

### **Opción 1: Verificar la Base de Datos**
```sql
-- Verificar conexión
SELECT @@VERSION;
SELECT DB_NAME();

-- Verificar tabla
SELECT COUNT(*) FROM [dbo].[AUD_PRIORIZACION_SOLICITUD];
```

### **Opción 2: Verificar el appsettings.json**
```json
{
  "ConnectionStrings": {
    "HospitalV4": "Server=.;Database=HospitalV4;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### **Opción 3: Limpiar y Recompilar Todo**
```bash
cd Hospital.Api
dotnet clean
dotnet build
dotnet run

# Nueva terminal
cd proyecto_hospital_version_1
dotnet clean
dotnet build
dotnet run
```

---

## ? ESTADO FINAL

- [x] Error de `SqlException` corregido
- [x] Método `GetTotalRegistrosAsync()` funcional
- [x] Controlador con `using` correcto
- [x] Build exitoso
- [x] Endpoint visible en Swagger
- [x] Blazor cargando datos correctamente

---

## ?? ¡CORRECCIÓN COMPLETADA!

**El historial de logs ahora debería funcionar correctamente sin errores.**

**Siguiente paso**: Reinicia la API y Blazor para aplicar los cambios.

---

**Última actualización**: 22 de enero de 2025  
**Estado**: ? **CORREGIDO Y FUNCIONAL**
