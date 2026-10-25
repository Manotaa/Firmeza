using Firmeza.Application.DTOs;

namespace Firmeza.Application.Interfaces;

public interface IVentaService
{
    Task<IEnumerable<VentaDto>> ObtenerTodasAsync(CancellationToken cancellationToken = default);
    Task<VentaDto?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<VentaDto?> ObtenerPorNumeroAsync(string numeroVenta, CancellationToken cancellationToken = default);
    Task<IEnumerable<VentaDto>> ObtenerPorClienteAsync(int clienteId, CancellationToken cancellationToken = default);
    Task<VentaDto> RegistrarVentaAsync(CrearVentaDto dto, CancellationToken cancellationToken = default);
}
