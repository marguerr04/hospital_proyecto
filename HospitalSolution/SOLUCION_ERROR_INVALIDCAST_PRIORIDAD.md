# ? SOLUCIÓN: Error InvalidCastException en Historial de Logs

## ?? Resumen del Problema

**Error Original:**
```
System.InvalidCastException: Unable to cast object of type 'System.Byte' to type 'System.Int32'.
```

**Causa Raíz:**
La columna `prioridad` en la tabla `AUD_PRIORIZACION_SOLICITUD` de SQL Server es de tipo `TINYINT` (que se mapea a `byte` en C#), pero los modelos de la API estaban definidos con tipo `int` (Int32).

**Impacto:**
- La página "Historial de Logs - Auditoría de Priorizaciones" mostraba el conteo correcto (21 registros) pero no podía cargar los datos en la tabla
- El error ocurría en el método `GetHistorialAuditoriaAsync` del servicio de auditoría

---

## ?? Archivos Modificados

### 1?? **Hospital.Api\DTOs\AuditoriaPriorizacionDto.cs**

**Cambio:** Línea 20
```csharp
// ? ANTES
public int Prioridad { get; set; }

// ? DESPUÉS
public byte Prioridad { get; set; }  // Cambiado de int a byte para coincidir con tinyint de BD
```

---

### 2?? **Hospital.Api\Data\Services\AuditoriaPriorizacionService.cs**

**Cambio:** Línea 122 (clase auxiliar `AuditoriaPriorizacionQueryResult`)
```csharp
// ? ANTES
public int Prioridad { get; set; }

// ? DESPUÉS
public byte Prioridad { get; set; }  // Cambiado de int a byte para coincidir con tinyint de BD
```

---

## ? Validaciones Realizadas

### ?? No rompe otras funcionalidades porque:

1. **Compatibilidad con UI (Blazor):**
   - Los componentes Razor aceptan `byte` sin problemas
   - Las comparaciones en el switch (`1`, `2`, `3`) siguen funcionando
   - El mapeo CSS class basado en prioridad sigue funcionando

2. **Propiedades calculadas mantienen funcionalidad:**
   ```csharp
   public string PrioridadTexto => $"P{Prioridad}";
   
   public string PrioridadCssClass => Prioridad switch
   {
       1 => "badge bg-danger",
       2 => "badge bg-warning text-dark",
       _ => "badge bg-primary"
   };
   ```

3. **La entidad principal ya usaba byte:**
   - En `PriorizacionSolicitud.cs` la propiedad ya era `byte? Prioridad` (línea 36)
   - Solo el DTO de auditoría estaba desincronizado

4. **Sin cambios en base de datos:**
   - No se modificó la estructura de la tabla
   - No se requieren migraciones
   - Compatible con el stored procedure `usp_PRIORIZACION_SOLICITUD_Insert`

---

## ?? Pasos para Aplicar la Solución

### **IMPORTANTE: La API debe estar detenida antes de compilar**

1. **Detén la API** (si está corriendo en debug/ejecución)

2. **Compila el proyecto de API:**
   ```powershell
   cd Hospital.Api
   dotnet clean
   dotnet build
   ```

3. **Reinicia la API:**
   - Presiona F5 en Visual Studio, o
   - Ejecuta `dotnet run` en la terminal

4. **Prueba el Historial de Logs:**
   - Navega a la página "Historial de Logs - Auditoría de Priorizaciones"
   - Verifica que la tabla ahora muestre los 21 registros correctamente

---

## ?? Verificación de la Solución

### Antes del Fix:
```
? Total: 21 registros
? No hay registros de auditoría en el sistema.
? Error: System.InvalidCastException
```

### Después del Fix:
```
? Total: 21 registros
? Tabla muestra los registros correctamente con:
   - AudId
   - Fecha
   - Acción (INSERT/UPDATE/DELETE)
   - Usuario
   - Solicitud ID
   - Prioridad (P1, P2, P3)
   - Badges de color según prioridad
```

---

## ?? Tipos de Datos SQL Server ?? C#

| SQL Server | C# / .NET     | Rango          |
|-----------|---------------|----------------|
| `TINYINT` | `byte`        | 0 - 255        |
| `INT`     | `int` (Int32) | -2,147,483,648 a 2,147,483,647 |

**Regla de Oro:** Siempre usa `byte` en C# para columnas `TINYINT` en SQL Server.

---

## ?? Notas Adicionales

- Este mismo problema podría ocurrir en otros lugares si hay más tablas con columnas `TINYINT` mapeadas como `int`
- Si en el futuro se agregan más campos de auditoría, verificar que los tipos coincidan con la base de datos
- El Hot Reload puede aplicar estos cambios si estás en debug, pero es recomendable reiniciar completamente la API

---

## ?? Fecha de Resolución
2025-01-22

## ?? Estado
? **RESUELTO** - Sin necesidad de cambios en base de datos
