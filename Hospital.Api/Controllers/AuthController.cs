using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hospital.Api.Data;
using Hospital.Api.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Hospital.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly HospitalDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(HospitalDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            Console.WriteLine($"🔐 Intento de login: {dto.Username}");
            
            // Buscar usuario
            var user = await _context.USUARIO.FirstOrDefaultAsync(u => u.Username == dto.Username);

            if (user == null)
            {
                Console.WriteLine($"❌ Usuario no encontrado: {dto.Username}");
                return Unauthorized(new { mensaje = "Usuario no encontrado" });
            }

            Console.WriteLine($"✅ Usuario encontrado: {user.Username}");
            Console.WriteLine($"📋 Hash almacenado (primeros 10 chars): {user.PasswordHash.Substring(0, 10)}...");

            // Verificar contraseña
            bool passwordValido = VerifyPassword(dto.Password, user.PasswordHash);
            Console.WriteLine($"🔓 Verificación de password: {(passwordValido ? "✅ VÁLIDO" : "❌ INVÁLIDO")}");

            if (!passwordValido)
            {
                return Unauthorized(new { mensaje = "Contraseña incorrecta" });
            }

            // Verificar si está activo
            if (!user.Activo)
            {
                Console.WriteLine($"⚠️ Usuario inactivo: {user.Username}");
                return Forbid();
            }

            // Generar token JWT
            var (token, expires) = GenerateJwtToken(user.Username, user.Rol);
            Console.WriteLine($"🎫 Token generado exitosamente para {user.Username}");

            return Ok(new
            {
                token,
                expiresAt = expires,
                username = user.Username,
                role = user.Rol
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"💥 Error en login: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return StatusCode(500, new { mensaje = $"Error en el servidor: {ex.Message}" });
        }
    }

    private (string, DateTime) GenerateJwtToken(string username, string role)
    {
        var key = _configuration["Jwt:Key"];
        
        // ✅ Validar que la clave JWT existe
        if (string.IsNullOrEmpty(key))
        {
            throw new InvalidOperationException("La clave JWT no está configurada en appsettings.json");
        }

        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:TokenExpiryMinutes"] ?? "60"));

        var claims = new[] {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)
        };

        var tokenDescriptor = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256)
        );

        var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        return (token, expires);
    }

    private bool VerifyPassword(string password, string storedHash)
    {
        try
        {
            // Detectar versión del hash y usar el método correcto
            if (storedHash.StartsWith("$2b$"))
            {
                // Hash Enhanced (2b)
                Console.WriteLine("🔍 Usando EnhancedVerify para hash $2b$");
                return BCrypt.Net.BCrypt.EnhancedVerify(password, storedHash);
            }
            else
            {
                // Hash Standard (2a)
                Console.WriteLine("🔍 Usando Verify estándar para hash $2a$");
                return BCrypt.Net.BCrypt.Verify(password, storedHash);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error verificando password: {ex.Message}");
            return false;
        }
    }

    [HttpPost("generate-hash")]
    public IActionResult GenerateHash([FromBody] string password)
    {
        try
        {
                // ✅ CORREGIDO: Eliminar espacios en la variable
            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            
            return Ok(new
            {
                password = password,
                hash = hash,
                hashType = hash.Substring(0, 4), // Mostrar tipo de hash ($2a$ o $2b$)
                mensaje = "Hash generado correctamente"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("test")]
    public async Task<IActionResult> Test()
    {
        var count = await _context.USUARIO.CountAsync();
        return Ok(new { mensaje = "API funcionando", usuariosEnBD = count });
    }

    [HttpGet("debug-usuarios")]
    public async Task<IActionResult> DebugUsuarios()
    {
        var usuarios = await _context.USUARIO
            .Select(u => new
            {
                u.Id,
                u.Username,
                HashPrefix = u.PasswordHash.Substring(0, 10),
                HashType = u.PasswordHash.Substring(0, 4),
                u.Rol,
                u.Activo
            })
            .ToListAsync();

        return Ok(usuarios);
    }

    [HttpPost("reset-passwords")]
    public async Task<IActionResult> ResetPasswords()
    {
        try
        {
            Console.WriteLine("🔧 RESETEANDO CONTRASEÑAS...");

            // Buscar usuarios
            var admin = await _context.USUARIO.FirstOrDefaultAsync(u => u.Username == "admin");
            var medico1 = await _context.USUARIO.FirstOrDefaultAsync(u => u.Username == "medico1");

            int cambios = 0;

            if (admin != null)
            {
                admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin");
                Console.WriteLine($"✅ Admin actualizado");
                cambios++;
            }

            if (medico1 != null)
            {
                medico1.PasswordHash = BCrypt.Net.BCrypt.HashPassword("medico");
                Console.WriteLine($"✅ Medico1 actualizado");
                cambios++;
            }

            await _context.SaveChangesAsync();
            Console.WriteLine($"💾 {cambios} usuarios actualizados");

            return Ok(new
            {
                mensaje = "✅ Listo! Usa admin/admin o medico1/medico",
                cambios
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}