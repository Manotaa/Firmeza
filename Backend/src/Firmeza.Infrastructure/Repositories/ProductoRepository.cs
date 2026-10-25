using Firmeza.Domain.Entities;
using Firmeza.Domain.Interfaces;
using Firmeza.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Infrastructure.Repositories;

public class ProductoRepository : IProductoRepository
{
    private readonly FirmezaDbContext _context;

    public ProductoRepository(FirmezaDbContext context)
    {
        _context = context;
    }

    public async Task<Producto?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Productos.FindAsync([id], cancellationToken);
    }

    public async Task<IEnumerable<Producto>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Productos
            .AsNoTracking()
            .OrderBy(p => p.Nombre)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Producto>> BuscarPorNombreAsync(string nombre, CancellationToken cancellationToken = default)
    {
        return await _context.Productos
            .AsNoTracking()
            .Where(p => EF.Functions.ILike(p.Nombre, $"%{nombre}%"))
            .OrderBy(p => p.Nombre)
            .ToListAsync(cancellationToken);
    }

    public async Task AgregarAsync(Producto producto, CancellationToken cancellationToken = default)
    {
        await _context.Productos.AddAsync(producto, cancellationToken);
    }

    public Task ActualizarAsync(Producto producto, CancellationToken cancellationToken = default)
    {
        _context.Productos.Update(producto);
        return Task.CompletedTask;
    }

    public async Task EliminarAsync(int id, CancellationToken cancellationToken = default)
    {
        var producto = await ObtenerPorIdAsync(id, cancellationToken);
        if (producto != null)
        {
            _context.Productos.Remove(producto);
        }
    }

    public async Task<bool> ExisteAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Productos.AnyAsync(p => p.Id == id, cancellationToken);
    }
}
