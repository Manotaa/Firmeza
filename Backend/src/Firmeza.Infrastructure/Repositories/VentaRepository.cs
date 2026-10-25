using Firmeza.Domain.Entities;
using Firmeza.Domain.Interfaces;
using Firmeza.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Infrastructure.Repositories;

public class VentaRepository : IVentaRepository
{
    private readonly FirmezaDbContext _context;

    public VentaRepository(FirmezaDbContext context)
    {
        _context = context;
    }

    public async Task<Venta?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<Venta?> ObtenerPorNumeroAsync(string numeroVenta, CancellationToken cancellationToken = default)
    {
        return await _context.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(v => v.NumeroVenta == numeroVenta, cancellationToken);
    }

    public async Task<IEnumerable<Venta>> ObtenerTodasAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Ventas
            .AsNoTracking()
            .Include(v => v.Cliente)
            .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
            .OrderByDescending(v => v.Fecha)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Venta>> ObtenerPorClienteAsync(int clienteId, CancellationToken cancellationToken = default)
    {
        return await _context.Ventas
            .AsNoTracking()
            .Where(v => v.ClienteId == clienteId)
            .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
            .OrderByDescending(v => v.Fecha)
            .ToListAsync(cancellationToken);
    }

    public async Task AgregarAsync(Venta venta, CancellationToken cancellationToken = default)
    {
        await _context.Ventas.AddAsync(venta, cancellationToken);
    }
}
