using Firmeza.Domain.Entities;

namespace Firmeza.Domain.Interfaces;

public interface IVentaRepository
{
    Task<Venta?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Venta?> ObtenerPorNumeroAsync(string numeroVenta, CancellationToken cancellationToken = default);
    Task<IEnumerable<Venta>> ObtenerTodasAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Venta>> ObtenerPorClienteAsync(int clienteId, CancellationToken cancellationToken = default);
    Task AgregarAsync(Venta venta, CancellationToken cancellationToken = default);
}
