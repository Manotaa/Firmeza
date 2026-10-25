using Firmeza.Application.DTOs;

namespace Firmeza.Application.Interfaces;

public interface IPdfReciboService
{
    Task<byte[]> GenerarReciboPdfAsync(VentaDto venta, CancellationToken cancellationToken = default);
    Task<string> GuardarReciboPdfAsync(VentaDto venta, string webRootPath, CancellationToken cancellationToken = default);
}
