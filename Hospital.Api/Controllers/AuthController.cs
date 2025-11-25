using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hospital.Api.Data;
using Hospital.Api.DTOs;

namespace Hospital.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly HospitalDbContext _context;

    public AuthController(HospitalDbContext context)
    {
        _context = context;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            // Validar que los campos no estén vacíos
            if (string.IsNullOrWhiteSpace(request.Username) || 
                string.IsNullOrWhiteSpace(request.Password) || 
                string.IsNullOrWhiteSpace(request.Rol))
            {
                return Ok(new LoginResponseDto
                {
                    Exito = false,
                    Mensaje = "Por favor, complete todos los campos"
                });
            }

            // Buscar usuario en la base de datos
            var usuario = await _context.USUARIO
                .FirstOrDefaultAsync(u => 
                    u.Username == request.Username && 
                    u.Rol == request.Rol && 
                    u.Activo);

            if (usuario == null)
            {
                return Ok(new LoginResponseDto
                {
                    Exito = false,
                    Mensaje = "Usuario, contraseña o rol incorrectos"
                });
            }

            // Verificar contraseña hasheada con BCrypt
            bool passwordValida = BCrypt.Net.BCrypt.EnhancedVerify(request.Password, usuario.PasswordHash);

            if (!passwordValida)
            {
                return Ok(new LoginResponseDto
                {
                    Exito = false,
                    Mensaje = "Usuario, contraseña o rol incorrectos"
                });
            }

            // Login exitoso
            return Ok(new LoginResponseDto
            {
                Exito = true,
                Mensaje = "Login exitoso",
                Username = usuario.Username,
                Rol = usuario.Rol
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new LoginResponseDto
            {
                Exito = false,
                Mensaje = $"Error en el servidor: {ex.Message}"
            });
        }
    }

    [HttpPost("generate-hash")]
    public IActionResult GenerateHash([FromBody] string password)
    {
        try
        {
            var hash = BCrypt.Net.BCrypt.EnhancedHashPassword(password);
            return Ok(new 
            { 
                password = password, 
                hash = hash,
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
}
