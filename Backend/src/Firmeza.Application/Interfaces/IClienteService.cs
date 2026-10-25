using Firmeza.Application.DTOs;

namespace Firmeza.Application.Interfaces;

public interface IClienteService
{
    Task<IEnumerable<ClienteDto>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ClienteDto>> BuscarAsync(string query, CancellationToken cancellationToken = default);
    Task<ClienteDto?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ClienteDto?> ObtenerPorDocumentoAsync(string documento, CancellationToken cancellationToken = default);
    Task<ClienteDto> CrearAsync(CrearClienteDto dto, CancellationToken cancellationToken = default);
    Task<ClienteDto?> ActualizarAsync(int id, ActualizarClienteDto dto, CancellationToken cancellationToken = default);
    Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default);
}
