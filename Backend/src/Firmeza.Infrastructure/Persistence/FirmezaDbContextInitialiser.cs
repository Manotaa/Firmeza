using Firmeza.Domain.Entities;
using Firmeza.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Firmeza.Infrastructure.Persistence;

public class FirmezaDbContextInitialiser
{
    private readonly ILogger<FirmezaDbContextInitialiser> _logger;
    private readonly FirmezaDbContext _context;
    private readonly UserManager<UsuarioFirmeza> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public FirmezaDbContextInitialiser(
        ILogger<FirmezaDbContextInitialiser> logger,
        FirmezaDbContext context,
        UserManager<UsuarioFirmeza> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (_context.Database.IsNpgsql())
            {
                await _context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocurrió un error al inicializar la base de datos.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocurrió un error al poblar los datos iniciales.");
            throw;
        }
    }

    private async Task TrySeedAsync()
    {
        // 1. Roles por defecto
        var roles = new[] { Roles.Admin, Roles.Cliente };
        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
                _logger.LogInformation("Rol creado: {Role}", role);
            }
        }

        // 2. Usuarios por defecto (Administradores y Clientes)
        var usuariosIniciales = new (string email, string nombre, string role, string pass)[]
        {
            ("admin@firmeza.com", "Administrador Firmeza", Roles.Admin, "Admin123*"),
            ("supervisor.obras@firmeza.com", "Supervisor de Obras", Roles.Admin, "Admin123*"),
            ("ventas.mostrador@firmeza.com", "Ventas Mostrador", Roles.Admin, "Admin123*"),
            ("carlos.mendoza@firmeza.com", "Carlos Mendoza", Roles.Cliente, "Password123*"),
            ("maria.gomez@firmeza.com", "María Fernanda Gómez", Roles.Cliente, "Password123*"),
            ("ferreteria.roble@firmeza.com", "Ferretería El Roble", Roles.Cliente, "Password123*"),
            ("consorcio.andino@firmeza.com", "Consorcio Vial Andino", Roles.Cliente, "Password123*")
        };

        foreach (var (email, nombre, role, pass) in usuariosIniciales)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new UsuarioFirmeza
                {
                    UserName = email,
                    Email = email,
                    NombreCompleto = nombre,
                    EmailConfirmed = true,
                    FechaRegistro = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, pass);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, role);
                    _logger.LogInformation("Usuario sembrado: {Email} con rol {Role}", email, role);
                }
            }
        }

        // 3. Productos iniciales si la tabla está vacía
        if (!await _context.Productos.AnyAsync())
        {
            _context.Productos.AddRange(
                new Producto { Nombre = "Cemento Gris 50kg", Precio = 28500m, Stock = 150 },
                new Producto { Nombre = "Varilla Corrugada 1/2 pulgada", Precio = 42000m, Stock = 80 },
                new Producto { Nombre = "Pintura Vinilo Blanco Galón", Precio = 55000m, Stock = 45 },
                new Producto { Nombre = "Ladrillo Estructural Tolete", Precio = 1200m, Stock = 1000 },
                new Producto { Nombre = "Arena Lavada Bulto", Precio = 8500m, Stock = 200 },
                new Producto { Nombre = "Taladro Percutor 1/2 Pulgada 650W", Precio = 189000m, Stock = 25 },
                new Producto { Nombre = "Teja Ondulada Fibrocemento N° 6", Precio = 48000m, Stock = 85 },
                new Producto { Nombre = "Tubo PVC Presión RDE 21 1/2 pulgada", Precio = 16500m, Stock = 150 },
                new Producto { Nombre = "Pegacor Corona Gris Bulto 25kg", Precio = 32000m, Stock = 110 },
                new Producto { Nombre = "Malla Electrosoldada 4mm 2.15x6m", Precio = 85000m, Stock = 50 },
                new Producto { Nombre = "Disco Corte Diamantado 4.5 pulgadas", Precio = 24500m, Stock = 95 },
                new Producto { Nombre = "Yeso de Construcción Bulto 25kg", Precio = 18000m, Stock = 70 }
            );
            await _context.SaveChangesAsync();
            _logger.LogInformation("12 productos iniciales sembrados en la base de datos.");
        }

        // 4. Clientes iniciales si la tabla está vacía
        if (!await _context.Clientes.AnyAsync())
        {
            _context.Clientes.AddRange(
                new Cliente { Documento = "1012345678", Nombre = "Constructora Bolívar SAS" },
                new Cliente { Documento = "900123456-1", Nombre = "Inversiones & Obras Civiles LTDA" },
                new Cliente { Documento = "1098765432", Nombre = "Juan Carlos Rodríguez" },
                new Cliente { Documento = "1032456789", Nombre = "Edificaciones y Estructuras del Norte SAS" },
                new Cliente { Documento = "830098765-4", Nombre = "Distribuidora Ferretera El Roble" },
                new Cliente { Documento = "1019876543", Nombre = "María Fernanda Gómez" },
                new Cliente { Documento = "901345678-2", Nombre = "Consorcio Vial Andino" },
                new Cliente { Documento = "1028765432", Nombre = "Andrés Felipe Morales" }
            );
            await _context.SaveChangesAsync();
            _logger.LogInformation("8 clientes iniciales sembrados en la base de datos.");
        }

        // 5. Ventas y detalles iniciales si la tabla de ventas está vacía
        if (!await _context.Ventas.AnyAsync())
        {
            var venta1 = new Venta
            {
                NumeroVenta = "FAC-20261020-001",
                Fecha = DateTime.UtcNow.AddDays(-4),
                ClienteId = 2,
                Subtotal = 990000m,
                Iva = 188100m,
                Total = 1178100m,
                Detalles = new List<DetalleVenta>
                {
                    new() { ProductoId = 2, Cantidad = 10, PrecioUnitario = 42000m, Subtotal = 420000m },
                    new() { ProductoId = 1, Cantidad = 20, PrecioUnitario = 28500m, Subtotal = 570000m }
                }
            };

            var venta2 = new Venta
            {
                NumeroVenta = "FAC-20261021-002",
                Fecha = DateTime.UtcNow.AddDays(-3),
                ClienteId = 3,
                Subtotal = 354000m,
                Iva = 67260m,
                Total = 421260m,
                Detalles = new List<DetalleVenta>
                {
                    new() { ProductoId = 3, Cantidad = 3, PrecioUnitario = 55000m, Subtotal = 165000m },
                    new() { ProductoId = 6, Cantidad = 1, PrecioUnitario = 189000m, Subtotal = 189000m }
                }
            };

            var venta3 = new Venta
            {
                NumeroVenta = "FAC-20261022-003",
                Fecha = DateTime.UtcNow.AddDays(-2),
                ClienteId = 4,
                Subtotal = 855000m,
                Iva = 162450m,
                Total = 1017450m,
                Detalles = new List<DetalleVenta>
                {
                    new() { ProductoId = 4, Cantidad = 500, PrecioUnitario = 1200m, Subtotal = 600000m },
                    new() { ProductoId = 5, Cantidad = 30, PrecioUnitario = 8500m, Subtotal = 255000m }
                }
            };

            var venta4 = new Venta
            {
                NumeroVenta = "FAC-20261023-004",
                Fecha = DateTime.UtcNow.AddDays(-1),
                ClienteId = 5,
                Subtotal = 1360000m,
                Iva = 258400m,
                Total = 1618400m,
                Detalles = new List<DetalleVenta>
                {
                    new() { ProductoId = 7, Cantidad = 15, PrecioUnitario = 48000m, Subtotal = 720000m },
                    new() { ProductoId = 9, Cantidad = 20, PrecioUnitario = 32000m, Subtotal = 640000m }
                }
            };

            var venta5 = new Venta
            {
                NumeroVenta = "FAC-20261024-005",
                Fecha = DateTime.UtcNow,
                ClienteId = 6,
                Subtotal = 230000m,
                Iva = 43700m,
                Total = 273700m,
                Detalles = new List<DetalleVenta>
                {
                    new() { ProductoId = 8, Cantidad = 8, PrecioUnitario = 16500m, Subtotal = 132000m },
                    new() { ProductoId = 11, Cantidad = 4, PrecioUnitario = 24500m, Subtotal = 98000m }
                }
            };

            _context.Ventas.AddRange(venta1, venta2, venta3, venta4, venta5);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Ventas y detalles iniciales sembrados en la base de datos.");
        }
    }
}
