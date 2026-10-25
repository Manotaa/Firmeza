using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Firmeza.Application.DTOs;
using Firmeza.Application.Interfaces;
using Firmeza.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Firmeza.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<UsuarioFirmeza> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AuthService(
        UserManager<UsuarioFirmeza> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Credenciales inválidas.");
        }

        var passwordValida = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!passwordValida)
        {
            throw new UnauthorizedAccessException("Credenciales inválidas.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var rolPrincipal = roles.FirstOrDefault() ?? Roles.Cliente;

        return GenerarToken(user, rolPrincipal);
    }

    public async Task<AuthResponseDto> RegistrarAsync(RegistroDto dto, string rol = Roles.Cliente, CancellationToken cancellationToken = default)
    {
        var userExistente = await _userManager.FindByEmailAsync(dto.Email);
        if (userExistente != null)
        {
            throw new InvalidOperationException("Ya existe un usuario con este correo electrónico.");
        }

        var user = new UsuarioFirmeza
        {
            UserName = dto.Email,
            Email = dto.Email,
            NombreCompleto = dto.NombreCompleto,
            FechaRegistro = DateTime.UtcNow,
            EmailConfirmed = true
        };

        var createResult = await _userManager.CreateAsync(user, dto.Password);
        if (!createResult.Succeeded)
        {
            var errores = string.Join("; ", createResult.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Error al registrar el usuario: {errores}");
        }

        if (!await _roleManager.RoleExistsAsync(rol))
        {
            await _roleManager.CreateAsync(new IdentityRole(rol));
        }

        await _userManager.AddToRoleAsync(user, rol);

        return GenerarToken(user, rol);
    }

    public async Task<UsuarioDto?> ObtenerUsuarioActualAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);
        return new UsuarioDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            NombreCompleto = user.NombreCompleto,
            Rol = roles.FirstOrDefault() ?? Roles.Cliente,
            FechaRegistro = user.FechaRegistro
        };
    }

    private AuthResponseDto GenerarToken(UsuarioFirmeza user, string rol)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? "ClaveSuperSecretaFirmeza2026_JWT_Key_Segura_123456";
        var jwtIssuer = _configuration["Jwt:Issuer"] ?? "FirmezaApi";
        var jwtAudience = _configuration["Jwt:Audience"] ?? "FirmezaClients";
        var duracionMinutos = int.TryParse(_configuration["Jwt:DurationInMinutes"], out var min) ? min : 1440;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expira = DateTime.UtcNow.AddMinutes(duracionMinutos);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.NombreCompleto),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Role, rol),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: expira,
            signingCredentials: creds
        );

        return new AuthResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Email = user.Email ?? string.Empty,
            NombreCompleto = user.NombreCompleto,
            Rol = rol,
            Expira = expira
        };
    }
}
