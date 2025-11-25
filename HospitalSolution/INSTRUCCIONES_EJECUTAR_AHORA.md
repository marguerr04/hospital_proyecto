# ?? INSTRUCCIONES RÁPIDAS - CONFIGURAR CONTRASEÑAS

## ? **LO QUE VOY A HACER POR TI:**

He corregido el script y creado `GENERAR_HASHES_SIMPLE.ps1` que:
- ? Verifica que la API esté corriendo
- ? Genera hashes BCrypt para "admin" y "medico"
- ? Crea automáticamente el script SQL `UPDATE_PASSWORDS_GENERATED.sql`
- ? Es compatible con todas las versiones de PowerShell

---

## ?? **EJECUTA ESTOS PASOS:**

### **1. Iniciar la API (Terminal 1)**
```powershell
cd E:\Retorno_cero\Testeo_recuperacion_1\Hospital.Api
dotnet run
```
**Deja esta ventana abierta. Espera a ver el mensaje "Now listening on: https://localhost:7032"**

---

### **2. Ejecutar el generador de hashes (Terminal 2)**
Abre **otra terminal de PowerShell** y ejecuta:

```powershell
cd E:\Retorno_cero\Testeo_recuperacion_1\HospitalSolution
.\GENERAR_HASHES_SIMPLE.ps1
```

**El script te pedirá:**
1. Confirmar que la API está corriendo (presiona ENTER)
2. Generará los hashes
3. Creará el archivo `UPDATE_PASSWORDS_GENERATED.sql`

---

### **3. Ejecutar el SQL generado**

#### **Opción A: Desde SQL Server Management Studio (SSMS)**
1. Abre **SQL Server Management Studio**
2. Conéctate a: `localhost\MSSQLSERVER01`
3. Menú: **Archivo ? Abrir ? Archivo**
4. Selecciona: `UPDATE_PASSWORDS_GENERATED.sql`
5. Presiona **F5** para ejecutar
6. Verifica que se actualizaron 4 registros

#### **Opción B: Desde la terminal**
```powershell
sqlcmd -S localhost\MSSQLSERVER01 -i UPDATE_PASSWORDS_GENERATED.sql
```

---

### **4. Probar el login**

#### **Iniciar Blazor (Terminal 3)**
```powershell
cd E:\Retorno_cero\Testeo_recuperacion_1\proyecto_hospital_version_1
dotnet run
```

#### **Abrir navegador**
Ve a: `https://localhost:XXXX/login` (el puerto lo verás en la terminal)

#### **Probar credenciales:**

| Usuario | Contraseña | Rol | Destino |
|---------|-----------|-----|---------|
| `admin` | `admin` | Administrador | `/dashboard` |
| `medico1` | `medico` | Medico | `/home-medico` |

---

## ?? **SI ALGO FALLA:**

### **Error: "No se pudo conectar con la API"**
- **Causa:** La API no está corriendo
- **Solución:** Asegúrate de ejecutar `dotnet run` en la carpeta `Hospital.Api`

### **Error: "Invalid object name 'dbo.USUARIO'"**
- **Causa:** La tabla no existe en la BD
- **Solución:** Verifica el nombre de tu base de datos en el script SQL

### **Error: "Invoke-RestMethod: No se puede conectar al servidor remoto"**
- **Causa:** Firewall o puerto bloqueado
- **Solución:** 
  ```powershell
  netsh advfirewall firewall add rule name="API Hospital" dir=in action=allow protocol=TCP localport=7032
  ```

---

## ?? **VERIFICACIÓN FINAL:**

Ejecuta este query en SQL Server para confirmar:

```sql
SELECT 
    id,
    username,
    rol,
    activo,
    LEFT(password_hash, 30) + '...' AS hash_preview,
    fecha_creacion
FROM [HospitalV4].[dbo].[USUARIO]
ORDER BY rol, username;
```

Deberías ver:
- ? 2 usuarios con rol **Administrador** (admin, admin2)
- ? 2 usuarios con rol **Medico** (medico1, medico2)
- ? Todos con el mismo hash según su rol
- ? Columna `activo` = 1

---

## ?? **¡LISTO PARA USAR!**

Después de estos pasos:
- ? Login funcional con base de datos
- ? Contraseñas hasheadas con BCrypt
- ? Autenticación por rol (Administrador/Medico)
- ? Redirección automática según rol

---

**?? Última actualización:** $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
