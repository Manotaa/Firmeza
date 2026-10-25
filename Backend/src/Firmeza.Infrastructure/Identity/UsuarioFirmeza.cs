using Microsoft.AspNetCore.Identity;

namespace Firmeza.Infrastructure.Identity;

public class UsuarioFirmeza : IdentityUser
{
    public string NombreCompleto { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
}
