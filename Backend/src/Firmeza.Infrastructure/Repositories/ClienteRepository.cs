using Firmeza.Domain.Entities;
using Firmeza.Domain.Interfaces;
using Firmeza.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Infrastructure.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly FirmezaDbContext _context;

    public ClienteRepository(FirmezaDbContext context)
    {
        _context = context;
    }

    public async Task<Cliente?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Clientes.FindAsync([id], cancellationToken);
    }

    public async Task<Cliente?> ObtenerPorDocumentoAsync(string documento, CancellationToken cancellationToken = default)
    {
        return await _context.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Documento == documento, cancellationToken);
    }

    public async Task<IEnumerable<Cliente>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Clientes
            .AsNoTracking()
            .OrderBy(c => c.Nombre)
            .ToListAsync(cancellationToken);
    }

    public async Task AgregarAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        await _context.Clientes.AddAsync(cliente, cancellationToken);
    }

    public Task ActualizarAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        _context.Clientes.Update(cliente);
        return Task.CompletedTask;
    }

    public async Task EliminarAsync(int id, CancellationToken cancellationToken = default)
    {
        var cliente = await ObtenerPorIdAsync(id, cancellationToken);
        if (cliente != null)
        {
            _context.Clientes.Remove(cliente);
        }
    }

    public async Task<bool> ExisteAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Clientes.AnyAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<bool> ExisteDocumentoAsync(string documento, CancellationToken cancellationToken = default)
    {
        return await _context.Clientes.AnyAsync(c => c.Documento == documento, cancellationToken);
    }
}
