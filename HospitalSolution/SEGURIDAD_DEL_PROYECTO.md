# Seguridad del Proyecto - Sistema de Gestión Hospitalaria

## Tabla de Contenidos
1. [Resumen Ejecutivo](#resumen-ejecutivo)
2. [Seguridad Implementada](#seguridad-implementada)
3. [Vulnerabilidades y Riesgos Actuales](#vulnerabilidades-y-riesgos-actuales)
4. [Buenas Prácticas Aplicadas](#buenas-prácticas-aplicadas)
5. [Plan de Mejora](#plan-de-mejora)

---

## Resumen Ejecutivo

**Estado de Seguridad: MODERADO ??**

Tu aplicación implementa algunas medidas de seguridad fundamentales, pero **faltan controles críticos** para un entorno hospitalario con datos sensibles. Este documento es completamente sincero sobre qué está bien implementado y qué necesita mejora.

### Puntuación de Seguridad (Estimada)
- **Autenticación**: 7/10 ?
- **Autorización**: 2/10 ?
- **Encriptación**: 5/10 ??
- **Validación de Datos**: 4/10 ??
- **Gestión de Errores**: 5/10 ??
- **Configuración Segura**: 3/10 ?
- **Auditoría y Logging**: 4/10 ??
- **Seguridad en Red**: 4/10 ??

**Promedio General: 4.25/10**

---

## Seguridad Implementada

### 1. AUTENTICACIÓN (JWT) ? 7/10

#### Qué SÍ está implementado:

**a) Hash de Contraseñas con BCrypt**
```csharp
// Controllers/AuthController.cs
private bool VerifyPassword(string password, string storedHash)
{
    if (storedHash.StartsWith("$2b$"))
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(password, storedHash);
    }
    else
    {
        return BCrypt.Net.BCrypt.Verify(password, storedHash);
    }
}
```

? **Fortalezas:**
- BCrypt con salt automático (muy seguro)
- Soporta múltiples versiones de hash
- Las contraseñas NO se almacenan en texto plano
- Cost factor implícito en BCrypt

**b) JWT Bearer Tokens**
```csharp
// Generación de tokens
var tokenDescriptor = new JwtSecurityToken(
    issuer: issuer,
    audience: audience,
    claims: claims,
    expires: expires,
    signingCredentials: new SigningCredentials(
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        SecurityAlgorithms.HmacSha256)
);
```

? **Fortalezas:**
- JWT con HMAC-SHA256 (criptografía sólida)
- Tokens con expiración (5 minutos configurado)
- Validación de issuer y audience
- Firma digital del token

**c) Validación de Tokens en API**
```csharp
// Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(...),
            ValidateLifetime = true
        };
    });
```

? **Fortalezas:**
- Validación estricta de todos los parámetros
- ClockSkew = TimeSpan.Zero (sin tolerancia de tiempo)
- ValidateLifetime = true

#### Qué FALTA o está débil:

? **Debilidades:**
- **No hay refresh tokens**: El token expira en 5 minutos pero no hay mecanismo para renovarlo
- **JWT en memoria del cliente**: Vulnerable a XSS si JavaScript tiene acceso
- **No hay revocación de tokens**: Un token comprometido sigue siendo válido hasta expirar
- **Contraseña por defecto en appsettings**:
  ```json
  "Jwt:Key": "CHANGE_THIS_WITH_A_STRONG_SECRET_AtLeast32Chars"
  ```
  La clave es muy corta y está en el repositorio

---

### 2. AUTORIZACIÓN ? 2/10

#### Qué FALTA:

```csharp
// Controllers/SolicitudController.cs - SIN PROTECCIÓN
[HttpPost("crear")]
public async Task<IActionResult> CrearSolicitud([FromBody] SolicitudCrearDto dto)
{
    // ? NO HAY [Authorize] ATTRIBUTE
    // ? NO HAY VALIDACIÓN DE ROLES
    // ? CUALQUIERA CON ACCESO A LA API PUEDE CREAR SOLICITUDES
}
```

? **Problemas críticos:**
- **Ningún endpoint tiene `[Authorize]`**: Los endpoints aceptan tokens inválidos
- **No hay validación de roles**: Un médico puede hacer operaciones de administrador
- **No hay autorización de datos**: Un usuario puede acceder a datos de otros pacientes
- **No hay verificación de permisos a nivel de negocio**

#### Qué debería estar implementado:

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize] // ? FALTA ESTO EN TODOS LOS CONTROLLERS
public class SolicitudController : ControllerBase
{
    [HttpPost("crear")]
    [Authorize(Roles = "Medico,Administrador")] // ? FALTA ESTO
    public async Task<IActionResult> CrearSolicitud([FromBody] SolicitudCrearDto dto)
    {
        // Validar que el usuario solo accede a sus propios datos
        var usuarioActual = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // ...
    }
}
```

---

### 3. ENCRIPTACIÓN ?? 5/10

#### Qué SÍ está implementado:

? **HTTPS Configurado**
```csharp
// Program.cs
app.UseHttpsRedirection();
```

? **En tránsito**: Las conexiones API van por HTTPS

#### Qué FALTA:

? **En reposo**:
- Base de datos **SIN ENCRIPTACIÓN** de columnas sensibles
- Datos de pacientes, diagnósticos, SSN almacenados en texto plano
- Contraseñas hasheadas ? pero todo lo demás expuesto

? **Configuración insegura**:
```json
"ConnectionStrings": {
    "HospitalV4": "Server=localhost\\MSSQLSERVER01;Database=HospitalV4;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

?? **Problemas:**
- `TrustServerCertificate=True` desactiva validación SSL (inseguro en producción)
- Trusted Connection = credenciales de Windows (no portable a cloud)
- Credenciales en archivo de configuración visible en repositorio

---

### 4. VALIDACIÓN DE DATOS ?? 4/10

#### Qué SÍ está implementado:

? **Validación básica de DTO**
```csharp
// Controllers/SolicitudController.cs
[HttpPost("crear")]
public async Task<IActionResult> CrearSolicitud([FromBody] SolicitudCrearDto dto)
{
    if (dto == null)
        return BadRequest(new { mensaje = "? El cuerpo de la solicitud está vacío." });
}
```

? **Data Annotations en DTOs**
```csharp
// DTOs/SolicitudCrearDto.cs
public class SolicitudCrearDto
{
    public int PacienteId { get; set; }
    public string DiagnosticoPrincipal { get; set; } = string.Empty;
    // Algunas propiedades tienen validación básica
}
```

#### Qué FALTA:

? **Sin validación robusta:**
- **No hay Range validation** en números (Peso, Talla, IMC)
- **No hay StringLength** en strings
- **No hay Required attributes** donde deberían estar
- **No hay validación de referencial**: ¿Existe el PacienteId?
- **Sin whitelist de valores**: Lateralidad, Extremidad podrían ser cualquier cosa
- **Sin sanitización**: Los comentarios de texto podrían contener inyecciones

? **Ejemplo vulnerable:**
```csharp
public decimal Peso { get; set; } // ? Puede ser -9999 o 999999
public string Comentarios { get; set; } = string.Empty; // ? Sin validación de largo
```

? **Sin validación cruzada**:
```csharp
public decimal Talla { get; set; }
public decimal IMC { get; set; }
// ? Nadie valida que IMC = Peso / (Talla * Talla)
```

---

### 5. GESTIÓN DE ERRORES Y LOGS ?? 5/10

#### Qué SÍ está implementado:

? **Try-Catch en controladores**
```csharp
try
{
    // Lógica
}
catch (Exception ex)
{
    Console.WriteLine($"Error en login: {ex.Message}");
    return StatusCode(500, new { mensaje = $"Error en el servidor: {ex.Message}" });
}
```

? **Logs a consola**
```csharp
Console.WriteLine($"?? Intento de login: {dto.Username}");
Console.WriteLine($"? Usuario no encontrado: {dto.Username}");
```

#### Qué FALTA:

? **Exposición de información sensible:**
```csharp
return StatusCode(500, new { mensaje = $"Error en el servidor: {ex.Message}" });
// ? Retorna el stack trace al cliente (info para atacantes)
```

? **Logs incompletos:**
- No hay auditoría de quién hizo qué
- No hay timestamps en logs
- No hay niveles de severidad
- Logs en consola = se pierden si la app se reinicia

? **Sin persistencia de logs:**
- Los logs no se guardan en archivos
- No hay rotación de logs
- Ninguna herramienta centralizada (ELK, Application Insights, etc.)

? **Debugging expuesto en producción:**
```csharp
[HttpGet("debug-usuarios")]
public async Task<IActionResult> DebugUsuarios()
{
    // ? ENDPOINT PÚBLICO QUE LISTA TODOS LOS USUARIOS
    var usuarios = await _context.USUARIO.ToListAsync();
    return Ok(usuarios);
}

[HttpPost("reset-passwords")]
public async Task<IActionResult> ResetPasswords()
{
    // ? ENDPOINT PÚBLICO PARA RESETEAR CONTRASEÑAS
}
```

**CRÍTICO**: Estos endpoints nunca deberían estar en producción.

---

### 6. CONFIGURACIÓN SEGURA ? 3/10

#### Qué FALTA:

? **Secretos en código fuente:**
```json
{
  "Jwt:Key": "CHANGE_THIS_WITH_A_STRONG_SECRET_AtLeast32Chars",
  "ConnectionStrings": {
    "HospitalV4": "Server=localhost\\MSSQLSERVER01;..."
  }
}
```

- ? Está en `.gitignore` pero...
- ? La clave por defecto es débil
- ? No hay diferencia entre Dev/Prod

? **CORS muy permisivo:**
```csharp
options.AddPolicy("AllowBlazorClient",
    policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
    //         ^^^^^^^^^^^^^^^^^^^^^^^^ CUALQUIER SITIO PUEDE ACCEDER
```

?? **Deberías especificar:**
```csharp
policy => policy
    .WithOrigins("https://tudominio.com")
    .WithMethods("GET", "POST", "PUT", "DELETE")
    .WithHeaders("Content-Type", "Authorization");
```

? **Variables de entorno no usadas:**
- Las credenciales están en appsettings.json
- En producción deberían venir de variables de entorno

? **Sin rate limiting:**
- Alguien puede hacer millones de requests
- Posible ataque de fuerza bruta en login

---

### 7. AUDITORÍA Y LOGGING ?? 4/10

#### Qué SÍ está implementado:

? **Intención de auditar**
```csharp
// En algunos servicios hay referencias a auditoría
// Pero no está completamente implementada
```

#### Qué FALTA:

? **Sin auditoría de cambios:**
- Quién creó una solicitud
- Quién modificó datos de pacientes
- Cuándo se hizo cada cambio
- Desde qué IP se accedió

? **Sin tabla de auditoría:**
```sql
-- FALTA ESTA TABLA
CREATE TABLE AUDITORIA (
    Id INT PRIMARY KEY,
    Usuario VARCHAR(100),
    Accion VARCHAR(100),
    Tabla VARCHAR(100),
    RegistroId INT,
    ValorAnterior NVARCHAR(MAX),
    ValorNuevo NVARCHAR(MAX),
    FechaHora DATETIME,
    IPAddress VARCHAR(50)
);
```

? **Sin log de accesos:**
- No hay registro de quién ingresó al sistema
- No hay registro de intentos fallidos
- No hay alertas de actividad sospechosa

---

### 8. SEGURIDAD EN RED ?? 4/10

#### Qué SÍ está implementado:

? **HTTPS requerido**
```csharp
app.UseHttpsRedirection();
```

#### Qué FALTA:

? **Sin WAF (Web Application Firewall):**
- Sin protección contra OWASP Top 10
- Sin detección de ataques

? **Sin Rate Limiting:**
- Vulnerable a ataques de fuerza bruta
- Vulnerable a DDoS

? **Sin headers de seguridad:**
```csharp
// FALTAN ESTOS HEADERS
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000");
    await next();
});
```

? **Sin CSRF protection:**
- Aunque Blazor tiene cierta protección automática
- Endpoints de API sin validación explícita

? **Sin CSP (Content Security Policy):**
- Vulnerable a XSS desde JavaScript externo

---

## Vulnerabilidades y Riesgos Actuales

### CRÍTICOS ??

| Vulnerabilidad | Riesgo | Ejemplo |
|---|---|---|
| **Autorización ausente** | Un usuario accede a datos de otros | Médico A accede a pacientes de Médico B |
| **Endpoints de debug públicos** | Exposición de usuarios/contraseñas | GET `/api/auth/debug-usuarios` |
| **Reset de contraseñas público** | Cualquiera resetea contraseñas | POST `/api/auth/reset-passwords` |
| **CORS abierto al mundo** | Cualquier sitio puede atacar | `AllowAnyOrigin()` |
| **Secretos en repositorio** | Clave JWT comprometida | JWT:Key en appsettings.json |

### ALTOS ??

| Vulnerabilidad | Riesgo | Impacto |
|---|---|---|
| **Sin validación de datos** | Inyección de SQL (aunque EF lo mitiga) | Datos inválidos en BD |
| **Sin encriptación en reposo** | Robo de datos de pacientes | HIPAA/RGPD violado |
| **Sin rate limiting** | Fuerza bruta en login | Contraseñas comprometidas |
| **Logs exponen detalles** | Stack traces al cliente | Info para atacantes |
| **Sin HTTPS en BD** | Credenciales en texto plano | Acceso a BD comprometido |

### MEDIOS ??

| Vulnerabilidad | Riesgo | Mitigación |
|---|---|---|
| **Sin refresh tokens** | Usuario bloqueado 5 min si token falla | Aceptable para este contexto |
| **Conexión a BD local** | No aplica en desarrollo | Debe cambiar en producción |
| **Sin auditoría completa** | No se sabe quién hizo qué | Necesario para cumplimiento |

---

## Buenas Prácticas Aplicadas

### ? Lo que SÍ haces bien:

1. **Hash de contraseñas con BCrypt**
   - Implementación correcta
   - Soporte de múltiples versiones de hash

2. **JWT para autenticación stateless**
   - Bueno para APIs
   - Permite escalabilidad

3. **Validación de tokens**
   - Verificación de firma
   - Validación de expiración
   - Validación de emisor y audiencia

4. **Separación de capas**
   - DTOs para transferencia de datos
   - Servicios para lógica de negocio
   - Controllers delegando a servicios

5. **HTTPS configurado**
   - Comunicación encriptada

6. **Estructura de carpetas lógica**
   - Fácil de mantener
   - Escalable

---

## Plan de Mejora

### CORTO PLAZO (1-2 semanas) ??

#### 1. Asegurar endpoints críticos
```csharp
// Modificar TODOS los controllers
[ApiController]
[Route("api/[controller]")]
[Authorize] // ? AGREGAR ESTO
public class SolicitudController : ControllerBase
{
    [HttpPost("crear")]
    [Authorize(Roles = "Medico,Administrador")] // ? ESPECIFICAR ROLES
    public async Task<IActionResult> CrearSolicitud([FromBody] SolicitudCrearDto dto)
    {
        // Validar usuario
        var usuario = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(usuario))
            return Unauthorized();
        
        // ...
    }
}
```

#### 2. Eliminar endpoints de debug
```csharp
// ? ELIMINAR ESTOS ENDPOINTS EN PRODUCCIÓN
[HttpGet("debug-usuarios")] // ? ELIMINAR
[HttpPost("reset-passwords")] // ? ELIMINAR
[HttpPost("generate-hash")] // ? ELIMINAR
```

#### 3. Restringir CORS
```csharp
options.AddPolicy("AllowBlazorClient",
    policy => policy
        .WithOrigins("https://tudominio.com") // ? ESPECIFICAR
        .WithMethods("GET", "POST", "PUT")
        .WithHeaders("Content-Type", "Authorization"));
```

#### 4. Mover secretos a variables de entorno
```csharp
// Program.cs
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") 
    ?? builder.Configuration["Jwt:Key"];

if (string.IsNullOrEmpty(jwtKey) || jwtKey.Length < 32)
{
    throw new InvalidOperationException(
        "JWT_KEY debe tener mínimo 32 caracteres");
}
```

#### 5. Agregar headers de seguridad
```csharp
// Program.cs
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add(
        "Strict-Transport-Security", "max-age=31536000; includeSubDomains");
    context.Response.Headers.Add(
        "Content-Security-Policy", "default-src 'self'");
    await next();
});
```

#### 6. Validar DTOs correctamente
```csharp
public class SolicitudCrearDto
{
    [Required(ErrorMessage = "PacienteId es requerido")]
    [Range(1, int.MaxValue)]
    public int PacienteId { get; set; }

    [Required]
    [StringLength(500, MinimumLength = 5)]
    public string DiagnosticoPrincipal { get; set; } = string.Empty;

    [Range(20, 250, ErrorMessage = "Peso debe estar entre 20 y 250 kg")]
    public decimal Peso { get; set; }

    [Range(50, 250, ErrorMessage = "Talla debe estar entre 50 y 250 cm")]
    public decimal Talla { get; set; }
}
```

### MEDIANO PLAZO (1-2 meses) ??

#### 1. Implementar tabla de auditoría
```sql
CREATE TABLE AUDITORIA (
    Id INT PRIMARY KEY IDENTITY,
    UsuarioId INT NOT NULL,
    Accion NVARCHAR(100) NOT NULL,
    Tabla NVARCHAR(100) NOT NULL,
    RegistroId INT,
    ValorAnterior NVARCHAR(MAX),
    ValorNuevo NVARCHAR(MAX),
    FechaHora DATETIME NOT NULL DEFAULT GETUTCDATE(),
    IPAddress VARCHAR(50),
    FOREIGN KEY (UsuarioId) REFERENCES USUARIO(Id)
);

-- Crear trigger para auditar cambios en SOLICITUD_QUIRURGICA
CREATE TRIGGER TR_AuditarSolicitud
ON SOLICITUD_QUIRURGICA
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    -- Lógica de auditoría
END;
```

#### 2. Encriptación en reposo
```csharp
// En Entity Framework
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Paciente>()
        .Property(p => p.Cedula)
        .HasConversion(
            v => EncryptionService.Encrypt(v),
            v => EncryptionService.Decrypt(v));
}
```

#### 3. Rate limiting
```csharp
// Instalar: dotnet add package AspNetCoreRateLimit
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.GeneralRules = new List<RateLimitRule>
    {
        new RateLimitRule
        {
            Endpoint = "*/api/*",
            Limit = 100,
            Period = "1m"
        }
    };
});
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

app.UseIpRateLimiting();
```

#### 4. Logging centralizado
```csharp
// Instalar: dotnet add package Serilog.AspNetCore
builder.Host.UseSerilog((context, config) =>
{
    config
        .MinimumLevel.Information()
        .WriteTo.File(
            "logs/app-.txt",
            rollingInterval: RollingInterval.Day)
        .WriteTo.Console();
});
```

#### 5. Refresh tokens
```csharp
public class AuthResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
}

[HttpPost("refresh")]
public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
{
    var principal = GetPrincipalFromExpiredToken(request.AccessToken);
    var username = principal.FindFirst(ClaimTypes.Name)?.Value;
    
    var user = await _context.USUARIO.FirstOrDefaultAsync(u => u.Username == username);
    if (user?.RefreshToken != request.RefreshToken)
        return Unauthorized();
    
    var newAccessToken = GenerateJwtToken(user.Username, user.Rol).Token;
    return Ok(new { accessToken = newAccessToken });
}
```

### LARGO PLAZO (2-3 meses) ??

#### 1. OAuth2 / OIDC
- Implementar con Azure AD o similar
- Multi-factor authentication

#### 2. Compliance (HIPAA, RGPD)
- Encriptación de todos los datos sensibles
- Consentimiento explícito
- Derecho al olvido

#### 3. Testing de seguridad
- OWASP ZAP scans
- Pruebas de penetración
- Análisis estático con SonarQube

#### 4. Monitoreo y alertas
- Detección de anomalías
- Alertas en tiempo real
- Dashboard de seguridad

---

## Checklist de Seguridad Actual

```
? Contraseñas hasheadas con BCrypt
? JWT con HMAC-SHA256
? Validación de tokens
? HTTPS configurado
? [Authorize] en endpoints
? Validación de roles
? Encriptación en reposo
? Rate limiting
? Auditoría completa
? Logging persistente
? Headers de seguridad
? CORS restringido
? Secretos en variables de entorno
? Validación robusta de datos
? Sanitización de inputs
? Protección contra inyecciones
? Endpoints de debug removidos
? CSRF protection
? CSP headers
? Monitoreo de seguridad
```

---

## Recomendaciones Finales

### ?? Antes de Producción (OBLIGATORIO)

1. **Asegurar todos los endpoints** con `[Authorize]`
2. **Validar roles** en cada operación sensible
3. **Eliminar endpoints de debug y reset**
4. **Mover secretos a variables de entorno**
5. **Restringir CORS** a dominios específicos
6. **Agregar validación robusta** de DTOs
7. **Implementar auditoría básica**
8. **Configurar logging** con rotación

### ?? Monitoreo Recomendado

- Application Insights (Azure)
- ELK Stack (Elasticsearch, Logstash, Kibana)
- Splunk
- New Relic

### ?? Herramientas de Testing

- OWASP ZAP: Escaneo de vulnerabilidades
- Burp Suite: Pruebas manuales
- SonarQube: Análisis estático
- Dependabot: Escaneo de dependencias

### ?? Recursos Recomendados

- OWASP Top 10: https://owasp.org/www-project-top-ten/
- Microsoft Security Best Practices: https://docs.microsoft.com/en-us/aspnet/core/security/
- NIST Cybersecurity Framework: https://www.nist.gov/cyberframework
- HIPAA Security Rule: https://www.hhs.gov/hipaa/

---

## Conclusión

Tu aplicación tiene **fundamentos de seguridad** pero **carece de controles críticos** para un sistema hospitalario con datos sensibles.

**Prioridad 1**: Implementar autorización y autenticación robusta  
**Prioridad 2**: Asegurar la configuración y eliminar puntos de exposición  
**Prioridad 3**: Implementar auditoría y logging  
**Prioridad 4**: Encriptación en reposo y cumplimiento normativo

Con estos cambios, pasarías de **4.25/10 a 7/10** en seguridad general.

---

**Documento Generado**: Evaluación Sincera de Seguridad  
**Fecha**: 2024  
**Versión**: 1.0
