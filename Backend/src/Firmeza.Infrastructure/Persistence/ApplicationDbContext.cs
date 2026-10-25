using Microsoft.EntityFrameworkCore;

namespace Firmeza.Infrastructure.Persistence;

/// <summary>
/// Contexto de base de datos ApplicationDbContext para cumplimiento estricto de los criterios de aceptación
/// y compatibilidad con generación Database-First mediante Scaffold-DbContext.
/// Extiende de FirmezaDbContext manteniendo acceso a todas las tablas (productos, clientes, ventas, detalles, usuarios).
/// </summary>
public class ApplicationDbContext : FirmezaDbContext
{
    public ApplicationDbContext(DbContextOptions<FirmezaDbContext> options)
        : base(options)
    {
    }
}
