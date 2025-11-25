using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("");
        Console.WriteLine("=====================================");
        Console.WriteLine("  GENERADOR DE HASHES BCRYPT");
        Console.WriteLine("=====================================");
        Console.WriteLine("");

        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://localhost:7032");
        
        // Ignorar certificados SSL en desarrollo
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };
        
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost:7032")
        };

        try
        {
            Console.WriteLine("[1/3] Verificando API...");
            var testResponse = await httpClient.GetAsync("/api/Auth/test");
            
            if (!testResponse.IsSuccessStatusCode)
            {
                Console.WriteLine("? ERROR: La API no responde correctamente");
                return;
            }
            
            var testContent = await testResponse.Content.ReadAsStringAsync();
            Console.WriteLine($"? API funcionando: {testContent}");
            Console.WriteLine("");

            Console.WriteLine("[2/3] Generando hash para 'admin'...");
            var adminContent = new StringContent("\"admin\"", Encoding.UTF8, "application/json");
            var adminResponse = await httpClient.PostAsync("/api/Auth/generate-hash", adminContent);
            var adminJson = await adminResponse.Content.ReadAsStringAsync();
            var adminResult = JsonSerializer.Deserialize<HashResult>(adminJson);
            Console.WriteLine($"? Hash generado: {adminResult.hash.Substring(0, 30)}...");
            Console.WriteLine("");

            Console.WriteLine("[3/3] Generando hash para 'medico'...");
            var medicoContent = new StringContent("\"medico\"", Encoding.UTF8, "application/json");
            var medicoResponse = await httpClient.PostAsync("/api/Auth/generate-hash", medicoContent);
            var medicoJson = await medicoResponse.Content.ReadAsStringAsync();
            var medicoResult = JsonSerializer.Deserialize<HashResult>(medicoJson);
            Console.WriteLine($"? Hash generado: {medicoResult.hash.Substring(0, 30)}...");
            Console.WriteLine("");
            Console.WriteLine("");

            Console.WriteLine("=====================================");
            Console.WriteLine("  HASHES GENERADOS EXITOSAMENTE");
            Console.WriteLine("=====================================");
            Console.WriteLine("");
            Console.WriteLine("?? HASH PARA 'admin':");
            Console.WriteLine(adminResult.hash);
            Console.WriteLine("");
            Console.WriteLine("?? HASH PARA 'medico':");
            Console.WriteLine(medicoResult.hash);
            Console.WriteLine("");
            Console.WriteLine("");

            // Generar script SQL
            var sqlScript = $@"-- =============================================
-- SCRIPT GENERADO AUTOMÁTICAMENTE
-- Fecha: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
-- =============================================

USE [HospitalV4];
GO

-- Ver usuarios actuales
SELECT id, username, rol, activo, 
       LEFT(password_hash, 30) + '...' AS hash_preview
FROM [dbo].[USUARIO]
ORDER BY rol, username;
GO

-- Actualizar contraseña de administradores a ""admin""
UPDATE [dbo].[USUARIO]
SET password_hash = '{adminResult.hash}'
WHERE rol = 'Administrador';
GO

-- Actualizar contraseña de médicos a ""medico""
UPDATE [dbo].[USUARIO]
SET password_hash = '{medicoResult.hash}'
WHERE rol = 'Medico';
GO

-- Verificar cambios
SELECT id, username, rol, activo,
       LEFT(password_hash, 30) + '...' AS hash_nuevo
FROM [dbo].[USUARIO]
ORDER BY rol, username;
GO

-- =============================================
-- CREDENCIALES ACTUALIZADAS:
-- =============================================
-- admin / admin2  ? Contraseña: admin  (Rol: Administrador)
-- medico1 / medico2 ? Contraseña: medico (Rol: Medico)
-- =============================================
";

            await File.WriteAllTextAsync("UPDATE_PASSWORDS_GENERATED.sql", sqlScript);
            Console.WriteLine("? Script SQL guardado en: UPDATE_PASSWORDS_GENERATED.sql");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("=====================================");
            Console.WriteLine("  SIGUIENTE PASO");
            Console.WriteLine("=====================================");
            Console.WriteLine("");
            Console.WriteLine("1. Abre SQL Server Management Studio");
            Console.WriteLine("2. Conéctate a: localhost\\MSSQLSERVER01");
            Console.WriteLine("3. Abre el archivo: UPDATE_PASSWORDS_GENERATED.sql");
            Console.WriteLine("4. Ejecuta el script (F5)");
            Console.WriteLine("");
            Console.WriteLine("Credenciales resultantes:");
            Console.WriteLine("  • admin    / admin  / Administrador");
            Console.WriteLine("  • medico1  / medico / Medico");
            Console.WriteLine("");
            Console.WriteLine("=====================================");
            Console.WriteLine("");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"? ERROR: {ex.Message}");
            Console.WriteLine($"Detalles: {ex}");
        }
    }
}

class HashResult
{
    public string password { get; set; }
    public string hash { get; set; }
    public string mensaje { get; set; }
}
