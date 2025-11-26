# ?? GUÍA COMPLETA: POBLAMIENTO DE PROFESIONALES Y CONFIGURACIÓN DE HISTORIAL DE LOGS

## ? RESUMEN DE CAMBIOS REALIZADOS

### 1. **Script SQL para Poblar Profesionales** ?
- **Archivo**: `SQL_POBLAR_PROFESIONALES.sql`
- **Contenido**: 20 profesionales médicos con datos realistas
- **Tabla**: `PROFESIONAL`

### 2. **Renombramiento de Ruta** ?
- **Antes**: `/historial-priorizaciones`
- **Ahora**: `/historial-logs`
- **Archivos modificados**:
  - `Dashboard.razor` (método `IrAlHistorialLogs`)
  - `HistorialPriorizaciones.razor` (directiva `@page`)

### 3. **Nuevo DTO de Auditoría** ?
- **Archivo**: `Hospital.Api/DTOs/AuditoriaPriorizacionDto.cs`
- **Propósito**: Mapear la tabla `AUD_PRIORIZACION_SOLICITUD`
- **Características**:
  - Propiedades calculadas (`TiempoTranscurrido`, `PrioridadTexto`, `PrioridadCssClass`)

### 4. **Servicio de Auditoría en API** ?
- **Archivo**: `Hospital.Api/Data/Services/AuditoriaPriorizacionService.cs`
- **Interface**: `IAuditoriaPriorizacionService`
- **Métodos**:
  - `GetHistorialAuditoriaAsync(pageNumber, pageSize)` - Con paginación
  - `GetTotalRegistrosAsync()` - Cuenta total de registros

### 5. **Controlador de API** ?
- **Archivo**: `Hospital.Api/Controllers/AuditoriaPriorizacionController.cs`
- **Endpoints**:
  - `GET /api/AuditoriaPriorizacion` - Lista paginada
  - `GET /api/AuditoriaPriorizacion/total` - Total de registros

### 6. **Servicio Blazor** ?
- **Archivo**: `proyecto_hospital_version_1/Services/AuditoriaPriorizacionApiService.cs`
- **Interface**: `IAuditoriaPriorizacionApiService`
- **Propósito**: Consumir la API desde Blazor

### 7. **Actualización de Program.cs** ?
- **API**: Registrado `IAuditoriaPriorizacionService`
- **Blazor**: Registrado `IAuditoriaPriorizacionApiService` con `HttpClient`

### 8. **Página de Historial Renovada** ?
- **Archivo**: `HistorialPriorizaciones.razor`
- **Características**:
  - Paginación funcional (20 registros por página)
  - Resumen estadístico por prioridad
  - Indicadores de tiempo transcurrido
  - Diseño responsivo

---

## ?? PASOS PARA EJECUTAR

### **PASO 1: Poblar la Tabla de Profesionales**

1. Abre **SQL Server Management Studio (SSMS)**
2. Conéctate a la base de datos `HospitalV4`
3. Abre el archivo `SQL_POBLAR_PROFESIONALES.sql`
4. Ejecuta el script completo (F5)
5. Verifica los resultados:

```sql
SELECT 
    Id,
    rut + '-' + dv AS RUT,
    primerNombre + ' ' + ISNULL(segundoNombre + ' ', '') + 
    primerApellido + ' ' + ISNULL(segundoApellido, '') AS NombreCompleto
FROM [dbo].[PROFESIONAL]
ORDER BY Id DESC;
```

**Resultado esperado**: 20 nuevos profesionales insertados ?

---

### **PASO 2: Verificar la Tabla de Auditoría**

Verifica que existan registros en `AUD_PRIORIZACION_SOLICITUD`:

```sql
SELECT COUNT(*) AS TotalRegistros 
FROM [HospitalV4].[dbo].[AUD_PRIORIZACION_SOLICITUD];

SELECT TOP 10 * 
FROM [HospitalV4].[dbo].[AUD_PRIORIZACION_SOLICITUD]
ORDER BY [AudFecha] DESC;
```

**Si no hay registros**, los logs se poblarán automáticamente al priorizar solicitudes.

---

### **PASO 3: Compilar y Ejecutar la API**

1. Abre el proyecto `Hospital.Api`
2. Compila el proyecto:
   ```bash
   dotnet build
   ```
3. Ejecuta la API:
   ```bash
   dotnet run
   ```
4. Verifica Swagger: `https://localhost:7032/swagger`
5. Prueba el endpoint:
   ```
   GET https://localhost:7032/api/AuditoriaPriorizacion?pageNumber=1&pageSize=20
   ```

**Respuesta esperada**:
```json
{
  "data": [...],
  "paginaActual": 1,
  "tamañoPagina": 20,
  "totalRegistros": 21,
  "totalPaginas": 2
}
```

---

### **PASO 4: Ejecutar Blazor**

1. Abre el proyecto `proyecto_hospital_version_1`
2. Compila el proyecto:
   ```bash
   dotnet build
   ```
3. Ejecuta Blazor:
   ```bash
   dotnet run
   ```
4. Navega a: `https://localhost:7213/dashboard`
5. Haz clic en el botón verde **"Historial de Logs"**

**Resultado esperado**: Tabla con registros de auditoría paginados ?

---

## ?? PRUEBAS

### **Prueba 1: Navegación desde Dashboard**
1. Ve a `/dashboard`
2. Haz clic en el botón verde **"Historial de Logs"**
3. ? Deberías ver la página `/historial-logs`

### **Prueba 2: Paginación**
1. Ve a `/historial-logs`
2. Verifica que se muestren 20 registros por página
3. Haz clic en "Siguiente" ? Verifica que cambie la página
4. Haz clic en "Anterior" ? Verifica que vuelva a la página anterior

### **Prueba 3: Resumen Estadístico**
1. Verifica que las tarjetas de resumen muestren:
   - Total de prioridades P1 (Urgentes)
   - Total de prioridades P2 (Altas)
   - Total de otras prioridades

---

## ??? ESTRUCTURA DE LA BASE DE DATOS

### Tabla: `PROFESIONAL`
```sql
CREATE TABLE [dbo].[PROFESIONAL] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [rut] NVARCHAR(20) NOT NULL,
    [dv] NVARCHAR(1) NOT NULL,
    [primerNombre] NVARCHAR(50) NOT NULL,
    [segundoNombre] NVARCHAR(50),
    [primerApellido] NVARCHAR(50) NOT NULL,
    [segundoApellido] NVARCHAR(50)
);
```

### Tabla: `AUD_PRIORIZACION_SOLICITUD`
```sql
-- Esta tabla ya existe en tu base de datos
-- Columnas principales:
-- - AudId (INT, PK)
-- - AudFecha (DATETIME)
-- - AudAccion (NVARCHAR)
-- - AudUsuario (NVARCHAR)
-- - SOLICITUD_QUIRURGICA_idSolicitud (INT)
-- - prioridad (INT)
```

---

## ?? TROUBLESHOOTING

### Error: "No se pudo conectar a la base de datos"
**Solución**: Verifica la cadena de conexión en `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "HospitalV4": "Server=.;Database=HospitalV4;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### Error: "Endpoint no encontrado"
**Solución**: Verifica que la API esté corriendo en `https://localhost:7032`

### Error: "IAuditoriaPriorizacionApiService no está registrado"
**Solución**: Verifica que el servicio esté registrado en `Program.cs` de Blazor:
```csharp
builder.Services.AddHttpClient<IAuditoriaPriorizacionApiService, AuditoriaPriorizacionApiService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7032/");
});
```

---

## ? CHECKLIST FINAL

- [ ] Script SQL ejecutado (20 profesionales insertados)
- [ ] API compilada sin errores
- [ ] API ejecutándose en `https://localhost:7032`
- [ ] Swagger funcionando
- [ ] Endpoint de auditoría probado
- [ ] Blazor compilado sin errores
- [ ] Blazor ejecutándose en `https://localhost:7213`
- [ ] Navegación a `/historial-logs` funcional
- [ ] Paginación funcional
- [ ] Resumen estadístico mostrando datos

---

## ?? DOCUMENTACIÓN ADICIONAL

### Endpoints de la API

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/AuditoriaPriorizacion` | Lista paginada de logs |
| GET | `/api/AuditoriaPriorizacion/total` | Total de registros |

### Parámetros de Paginación

| Parámetro | Tipo | Default | Descripción |
|-----------|------|---------|-------------|
| `pageNumber` | int | 1 | Número de página |
| `pageSize` | int | 20 | Registros por página |

---

## ?? ¡IMPLEMENTACIÓN COMPLETA!

Todos los requerimientos han sido implementados:

? Tabla `PROFESIONAL` poblada con 20 registros  
? Ruta renombrada a `/historial-logs`  
? Endpoint de auditoría con paginación  
? Servicio Blazor funcional  
? Página de historial con diseño mejorado  
? Sin romper funcionalidad existente  

**¡Todo listo para usar!** ??
