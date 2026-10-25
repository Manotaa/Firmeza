using Firmeza.Domain.Entities;

namespace Firmeza.Domain.Interfaces;

public interface IProductoRepository
{
    Task<Producto?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Producto>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Producto>> BuscarPorNombreAsync(string nombre, CancellationToken cancellationToken = default);
    Task AgregarAsync(Producto producto, CancellationToken cancellationToken = default);
    Task ActualizarAsync(Producto producto, CancellationToken cancellationToken = default);
    Task EliminarAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExisteAsync(int id, CancellationToken cancellationToken = default);
}
