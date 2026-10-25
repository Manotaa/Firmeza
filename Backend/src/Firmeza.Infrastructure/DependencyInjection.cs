using Firmeza.Domain.Interfaces;
using Firmeza.Infrastructure.Identity;
using Firmeza.Infrastructure.Persistence;
using Firmeza.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Firmeza.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? "Host=localhost;Port=5432;Database=firmeza_db;Username=firmeza_user;Password=firmeza_pass";

        services.AddDbContext<FirmezaDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<ApplicationDbContext>(sp =>
            new ApplicationDbContext(sp.GetRequiredService<DbContextOptions<FirmezaDbContext>>()));

        services.AddIdentityCore<UsuarioFirmeza>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
            options.User.RequireUniqueEmail = true;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<FirmezaDbContext>();

        // Repositorios y Unit of Work
        services.AddScoped<IProductoRepository, ProductoRepository>();
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IVentaRepository, VentaRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Servicios de infraestructura (Auth JWT, Excel, PDF)
        services.AddScoped<Firmeza.Application.Interfaces.IAuthService, Firmeza.Infrastructure.Services.AuthService>();
        services.AddScoped<Firmeza.Application.Interfaces.IExcelService, Firmeza.Infrastructure.Services.ExcelService>();
        services.AddScoped<Firmeza.Application.Interfaces.IPdfReciboService, Firmeza.Infrastructure.Services.PdfReciboService>();

        // Inicializador de base de datos y seeding
        services.AddScoped<FirmezaDbContextInitialiser>();

        return services;
    }
}
