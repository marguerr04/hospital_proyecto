# ?? SOLUCIÓN COMPLETA - Errores HttpClient y MudBlazor

## ?? **DIAGNÓSTICO DE ERRORES**

### **Errores Encontrados:**

1. ? **HttpClient Disposed**
   ```
   Cannot access a disposed object.
   Object name: 'System.Net.Http.HttpClient'.
   ```

2. ? **DashboardService registrado dos veces**
   ```csharp
   builder.Services.AddScoped<DashboardService>(); // Línea 50
   builder.Services.AddScoped<DashboardService>(); // Línea 68 (duplicado)
   ```

3. ? **BaseUrl hardcodeada incorrecta**
   ```csharp
   private const string BaseUrl = "http://localhost:5227"; // ? Puerto incorrecto
   // Tu API está en: https://localhost:7032
   ```

4. ? **HttpClient sin configuración adecuada**
   ```csharp
   builder.Services.AddScoped(sp =>
   {
       var factory = sp.GetRequiredService<IHttpClientFactory>();
       return factory.CreateClient("ApiReal");
   });
   // Esto causa que el HttpClient se dispose prematuramente
   ```

---

## ? **SOLUCIONES APLICADAS**

### **1. Configuración Correcta de HttpClient en Program.cs**

**Antes (Incorrecto)**:
```csharp
builder.Services.AddHttpClient("ApiReal", client =>
{
    client.BaseAddress = new Uri("https://localhost:7032/");
});

builder.Services.AddScoped(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    return factory.CreateClient("ApiReal");
});

builder.Services.AddScoped<DashboardService>(); // Duplicado 1
// ...
builder.Services.AddScoped<DashboardService>(); // Duplicado 2
```

**Después (Correcto)**:
```csharp
// ? HttpClient configurado específicamente para cada servicio
builder.Services.AddHttpClient<DashboardService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7032/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<IPacienteApiService, PacienteApiService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7032/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<ISolicitudQuirurgicaApiService, SolicitudQuirurgicaApiService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7032/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// ... y así para cada servicio
```

**Ventajas**:
- ? HttpClient no se dispose prematuramente
- ? Cada servicio tiene su propio HttpClient configurado
- ? Timeout configurado para evitar cuelgues
- ? BaseAddress configurado en Program.cs (no hardcodeado)

---

### **2. Eliminar BaseUrl Hardcodeada de DashboardService.cs**

**Antes (Incorrecto)**:
```csharp
public class DashboardService
{
    private readonly HttpClient _http;
    private const string BaseUrl = "http://localhost:5227"; // ? Puerto incorrecto

    public DashboardService(HttpClient http)
    {
        _http = http;
    }
    // ...
}
```

**Después (Correcto)**:
```csharp
public class DashboardService
{
    private readonly HttpClient _http;

    public DashboardService(HttpClient http)
    {
        _http = http; // ? BaseAddress ya viene configurado desde Program.cs
    }
    // ...
}
```

**Ventajas**:
- ? BaseAddress centralizado en `Program.cs`
- ? Más fácil cambiar el puerto de la API
- ? Menos errores por puertos incorrectos

---

### **3. Eliminar Duplicación de DashboardService**

**Antes**:
```csharp
builder.Services.AddScoped<DashboardService>(); // Línea 50
// ...
builder.Services.AddScoped<DashboardService>(); // Línea 68 ?
```

**Después**:
```csharp
builder.Services.AddHttpClient<DashboardService>(client => ...); // ? Una sola vez
```

---

## ?? **PASOS PARA APLICAR LOS CAMBIOS**

### **IMPORTANTE**: Debes **REINICIAR** la aplicación porque:
- Hot Reload **NO puede eliminar constantes** (`const BaseUrl`)
- Los cambios en `Program.cs` requieren reinicio

### **Paso 1: Detener Ambas Aplicaciones**
```bash
# En terminal de la API (Ctrl+C)
# En terminal de Blazor (Ctrl+C)
```

### **Paso 2: Limpiar y Reconstruir**
```bash
# En proyecto Blazor
cd proyecto_hospital_version_1
dotnet clean
dotnet build

# En proyecto API
cd ../Hospital.Api
dotnet clean
dotnet build
```

### **Paso 3: Reiniciar la API Primero**
```bash
cd Hospital.Api
dotnet run
```

**Verificar que esté corriendo**:
- Terminal debe decir: `Now listening on: https://localhost:7032`
- Navegar a: `https://localhost:7032/swagger`
- Verificar que Swagger cargue correctamente

### **Paso 4: Reiniciar Blazor**
```bash
cd proyecto_hospital_version_1
dotnet run
```

**Verificar que esté corriendo**:
- Terminal debe decir: `Now listening on: http://localhost:XXXX`
- Navegar a: `http://localhost:XXXX/dashboard_verdadero`

---

## ?? **VERIFICACIÓN POST-REINICIO**

### **1. Verificar que NO haya errores HttpClient**
Revisa la consola de Blazor, **NO deberías ver**:
```
? Cannot access a disposed object.
? Object name: 'System.Net.Http.HttpClient'.
```

### **2. Verificar que los KPIs carguen**
En el dashboard deberías ver:
```
? Percentil 75: [número]
? Reducción: 25%
? Pendientes: [número]
```

### **3. Verificar que los gráficos carguen**
En la consola del navegador (F12) deberías ver:
```
? Procedimientos: [número > 0]
? Contactabilidad: [número > 0]
? Evolución: [número > 0]
? Causales: [número > 0]
```

### **4. Verificar Swagger**
Navega a `https://localhost:7032/swagger` y verifica:
- ? La página carga sin error 500
- ? Puedes expandir los endpoints
- ? Puedes probar `/api/dashboard/percentil75`

---

## ?? **ARQUITECTURA CORRECTA DE HTTPCLIENT**

### **Antes (Problemática)**:
```
Program.cs
  ?? AddHttpClient("ApiReal") ? HttpClient genérico
  ?? AddScoped<DashboardService>() ? NO recibe HttpClient configurado

DashboardService.cs
  ?? private const BaseUrl = "http://localhost:5227" ? Puerto incorrecto
```

### **Después (Correcta)**:
```
Program.cs
  ?? AddHttpClient<DashboardService>(...)
  ?    ?? BaseAddress: https://localhost:7032
  ?    ?? Timeout: 30s
  ?? AddHttpClient<PacienteApiService>(...)
  ?    ?? BaseAddress: https://localhost:7032
  ?    ?? Timeout: 30s
  ?? ... (uno por cada servicio)

DashboardService.cs
  ?? HttpClient _http ? Ya viene configurado desde Program.cs
```

---

## ?? **ERRORES COMUNES POST-REINICIO**

### **Si Swagger sigue con error 500:**
1. Verifica que la API esté corriendo en `https://localhost:7032`
2. Revisa la consola de la API por errores
3. Verifica el connection string en `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "HospitalV4": "Server=.;Database=HospitalV4;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```

### **Si HttpClient sigue dando error:**
1. Asegúrate de haber **DETENIDO** la aplicación (no Hot Reload)
2. Ejecuta `dotnet clean && dotnet build`
3. Reinicia Visual Studio (si usas VS)

### **Si MudPopoverProvider sigue dando error:**
1. Verifica que `App.razor` tenga:
   ```razor
   <MudPopoverProvider />
   ```
2. Verifica que esté usando `InteractiveServer` render mode:
   ```razor
   <Routes @rendermode="InteractiveServer" />
   ```

---

## ? **CHECKLIST DE VERIFICACIÓN**

- [ ] API detenida y reiniciada
- [ ] Blazor detenido y reiniciado
- [ ] `dotnet clean && dotnet build` ejecutado en ambos proyectos
- [ ] Swagger accesible sin error 500
- [ ] Dashboard carga sin errores HttpClient
- [ ] KPIs muestran valores correctos
- [ ] Gráficos renderizan correctamente
- [ ] Consola del navegador sin errores (F12)
- [ ] Consola de Blazor sin errores de HttpClient

---

## ?? **RESUMEN DE CAMBIOS**

| Archivo | Cambio | Estado |
|---|---|---|
| `Program.cs` (Blazor) | HttpClient configurado correctamente con `AddHttpClient<T>` | ? Aplicado |
| `Program.cs` (Blazor) | Eliminada duplicación de `DashboardService` | ? Aplicado |
| `DashboardService.cs` | Eliminada constante `BaseUrl` hardcodeada | ? Aplicado |
| `DashboardService.cs` | HttpClient recibe BaseAddress desde Program.cs | ? Aplicado |

---

## ?? **SI LOS ERRORES PERSISTEN**

1. **Cierra Visual Studio completamente** (si lo usas)
2. **Elimina las carpetas `bin` y `obj`**:
   ```bash
   rm -rf bin obj
   ```
3. **Reconstruye desde cero**:
   ```bash
   dotnet clean
   dotnet restore
   dotnet build
   ```
4. **Reinicia ambas aplicaciones en orden**:
   - Primero API
   - Luego Blazor

---

## ?? **¡Listo!**

Después de seguir estos pasos:
1. ? HttpClient no se dispondrá prematuramente
2. ? Swagger funcionará correctamente
3. ? Dashboard cargará datos reales
4. ? No habrá errores en consola

**Reinicia ambas aplicaciones y verifica que todo funcione!** ??
