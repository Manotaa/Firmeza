using Firmeza.Domain.Entities;

namespace Firmeza.Domain.Interfaces;

public interface IClienteRepository
{
    Task<Cliente?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Cliente?> ObtenerPorDocumentoAsync(string documento, CancellationToken cancellationToken = default);
    Task<IEnumerable<Cliente>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task AgregarAsync(Cliente cliente, CancellationToken cancellationToken = default);
    Task ActualizarAsync(Cliente cliente, CancellationToken cancellationToken = default);
    Task EliminarAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExisteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExisteDocumentoAsync(string documento, CancellationToken cancellationToken = default);
}
