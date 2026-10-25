using Firmeza.Application.DTOs;

namespace Firmeza.Application.Interfaces;

public interface IProductoService
{
    Task<IEnumerable<ProductoDto>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task<ProductoDto?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductoDto>> BuscarAsync(string query, CancellationToken cancellationToken = default);
    Task<ProductoDto> CrearAsync(CrearProductoDto dto, CancellationToken cancellationToken = default);
    Task<ProductoDto?> ActualizarAsync(int id, ActualizarProductoDto dto, CancellationToken cancellationToken = default);
    Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default);
}
