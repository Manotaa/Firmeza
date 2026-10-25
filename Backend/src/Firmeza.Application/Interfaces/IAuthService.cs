using Firmeza.Application.DTOs;

namespace Firmeza.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> RegistrarAsync(RegistroDto dto, string rol = "Cliente", CancellationToken cancellationToken = default);
    Task<UsuarioDto?> ObtenerUsuarioActualAsync(string userId, CancellationToken cancellationToken = default);
}

public class UsuarioDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; }
}
