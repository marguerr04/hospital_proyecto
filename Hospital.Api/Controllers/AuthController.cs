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
        // reutiliza la lógica de validación de usuario que ya tienes
        var user = await _context.USUARIO.FirstOrDefaultAsync(u => u.Username == dto.Username);
        if (user == null || !VerifyPassword(dto.Password, user.PasswordHash)) return Unauthorized();

        if (!user.IsActive) return Forbid();

        var (token, expires) = GenerateJwtToken(user.Username, user.Role);
        return Ok(new { token, expiresAt = expires, username = user.Username, role = user.Role });
    }

    private (string, DateTime) GenerateJwtToken(string username, string role)
    {
        var key = _configuration["Jwt:Key"];
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:TokenExpiryMinutes"] ?? "5"));

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
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256)
        );

        var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        return (token, expires);
    }

    private bool VerifyPassword(string password, string storedHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, storedHash);
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
