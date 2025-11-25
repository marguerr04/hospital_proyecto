# ?? GUÍA: ACTUALIZAR CONTRASEÑAS EN BASE DE DATOS

## ?? **Resumen**
Esta guía te ayudará a actualizar las contraseñas hasheadas en la base de datos para que las contraseñas sean:
- **admin** ? para usuarios administradores
- **medico** ? para usuarios médicos

---

## ?? **Paso 1: Iniciar la API**

```powershell
cd E:\Retorno_cero\Testeo_recuperacion_1\Hospital.Api
dotnet run
```

La API debe estar corriendo en `https://localhost:7032`

---

## ?? **Paso 2: Generar hashes con Postman/curl**

### **Opción A: Usar Postman**

1. **Método:** POST
2. **URL:** `https://localhost:7032/api/Auth/generate-hash`
3. **Headers:** 
   - `Content-Type: application/json`
4. **Body (raw JSON):**

Para generar hash de "admin":
```json
"admin"
```

Para generar hash de "medico":
```json
"medico"
```

### **Opción B: Usar curl desde PowerShell**

```powershell
# Generar hash para "admin"
curl -k -X POST https://localhost:7032/api/Auth/generate-hash `
  -H "Content-Type: application/json" `
  -d '"admin"'

# Generar hash para "medico"
curl -k -X POST https://localhost:7032/api/Auth/generate-hash `
  -H "Content-Type: application/json" `
  -d '"medico"'
```

### **Opción C: Usar script C# directo**

Si prefieres, aquí está el código para generar los hashes localmente:

```csharp
// Ejecutar en una consola C# o LINQPad
using BCrypt.Net;

var hashAdmin = BCrypt.EnhancedHashPassword("admin");
var hashMedico = BCrypt.EnhancedHashPassword("medico");

Console.WriteLine($"Hash para 'admin': {hashAdmin}");
Console.WriteLine($"Hash para 'medico': {hashMedico}");
```

---

## ?? **Paso 3: Actualizar la base de datos**

Una vez que tengas los hashes generados, ejecuta estos scripts SQL:

```sql
-- ========================================
-- ACTUALIZAR CONTRASEÑAS DE ADMINISTRADORES
-- ========================================
UPDATE [HospitalV4].[dbo].[USUARIO]
SET password_hash = '<HASH_GENERADO_PARA_ADMIN>'
WHERE rol = 'Administrador';

-- Verificar
SELECT id, username, rol, 
       LEFT(password_hash, 20) + '...' AS hash_preview
FROM [HospitalV4].[dbo].[USUARIO]
WHERE rol = 'Administrador';

-- ========================================
-- ACTUALIZAR CONTRASEÑAS DE MÉDICOS
-- ========================================
UPDATE [HospitalV4].[dbo].[USUARIO]
SET password_hash = '<HASH_GENERADO_PARA_MEDICO>'
WHERE rol = 'Medico';

-- Verificar
SELECT id, username, rol, 
       LEFT(password_hash, 20) + '...' AS hash_preview
FROM [HospitalV4].[dbo].[USUARIO]
WHERE rol = 'Medico';
```

### **Ejemplo con hashes reales (los tuyos serán diferentes):**

```sql
-- EJEMPLO (usa los hashes que generes, no estos)
UPDATE [HospitalV4].[dbo].[USUARIO]
SET password_hash = '$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy'
WHERE rol = 'Administrador';

UPDATE [HospitalV4].[dbo].[USUARIO]
SET password_hash = '$2a$11$K1wHtZqF6JwC8Y0lPxQZ7.oJ8x9L8xM7N9qF1K2L3M4N5O6P7Q8R9S'
WHERE rol = 'Medico';
```

---

## ? **Paso 4: Probar el login**

### **Credenciales de prueba:**

| Usuario | Contraseña | Rol | Página de destino |
|---------|-----------|-----|-------------------|
| `admin` | `admin` | Administrador | `/dashboard` |
| `admin2` | `admin` | Administrador | `/dashboard` |
| `medico1` | `medico` | Medico | `/home-medico` |
| `medico2` | `medico` | Medico | `/home-medico` |

### **Cómo probar:**

1. Iniciar la API:
```powershell
cd E:\Retorno_cero\Testeo_recuperacion_1\Hospital.Api
dotnet run
```

2. Iniciar Blazor:
```powershell
cd E:\Retorno_cero\Testeo_recuperacion_1\proyecto_hospital_version_1
dotnet run
```

3. Ir a `https://localhost:XXXX/login` (el puerto lo verás en la consola)

4. Ingresar:
   - **Usuario:** medico1
   - **Contraseña:** medico
   - **Rol:** Médico
   - **Resultado esperado:** Redirige a `/home-medico`

---

## ?? **Troubleshooting**

### **Error: "Usuario, contraseña o rol incorrectos"**

**Causa:** El hash en la BD no coincide con la contraseña ingresada.

**Solución:**
1. Verifica que ejecutaste el UPDATE correctamente
2. Genera nuevamente el hash usando el endpoint `/generate-hash`
3. Verifica que el hash en la BD sea exactamente el generado (sin espacios extra)

### **Error: "Error al comunicarse con el servidor"**

**Causa:** La API no está corriendo o hay un problema de CORS.

**Solución:**
1. Verifica que la API esté corriendo en `https://localhost:7032`
2. Verifica que no haya errores en la consola de la API

### **Error: "Por favor, complete todos los campos"**

**Causa:** Algún campo del formulario está vacío.

**Solución:** Asegúrate de completar Usuario, Contraseña Y Rol.

---

## ?? **Verificación rápida con SQL**

```sql
-- Ver todos los usuarios con sus roles
SELECT 
    id,
    username,
    rol,
    activo,
    fecha_creacion,
    LEFT(password_hash, 30) + '...' AS hash_preview
FROM [HospitalV4].[dbo].[USUARIO]
ORDER BY rol, username;
```

---

## ?? **Notas importantes**

1. ?? **Los hashes BCrypt son únicos cada vez** - Aunque uses la misma contraseña, cada hash será diferente (esto es normal y seguro).

2. ? **Verificación con BCrypt.Verify()** - El método `EnhancedVerify()` compara correctamente incluso si los hashes son diferentes.

3. ?? **Seguridad** - Nunca guardes contraseñas en texto plano. Siempre usa hashes BCrypt.

4. ??? **Endpoint temporal** - Recuerda eliminar o comentar el endpoint `/generate-hash` en producción.

---

## ?? **Resultado esperado**

Después de seguir esta guía:
- ? Puedes hacer login con `admin` / `admin` / `Administrador`
- ? Puedes hacer login con `medico1` / `medico` / `Medico`
- ? El sistema te redirige correctamente según tu rol
- ? El estado de autenticación se guarda en `AuthStateService`

---

**?? Fecha de creación:** 25/11/2025  
**?? Versión:** 1.0  
**?? Para:** Usuario hospitalario
