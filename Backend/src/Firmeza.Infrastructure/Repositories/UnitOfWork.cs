using Firmeza.Domain.Interfaces;
using Firmeza.Infrastructure.Persistence;

namespace Firmeza.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly FirmezaDbContext _context;
    private IProductoRepository? _productos;
    private IClienteRepository? _clientes;
    private IVentaRepository? _ventas;

    public UnitOfWork(FirmezaDbContext context)
    {
        _context = context;
    }

    public IProductoRepository Productos => _productos ??= new ProductoRepository(_context);
    public IClienteRepository Clientes => _clientes ??= new ClienteRepository(_context);
    public IVentaRepository Ventas => _ventas ??= new VentaRepository(_context);

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
