using Firmeza.Application.DTOs;
using Firmeza.Application.Interfaces;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Interfaces;

namespace Firmeza.Application.Services;

public class VentaService : IVentaService
{
    private readonly IUnitOfWork _unitOfWork;

    public VentaService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<VentaDto>> ObtenerTodasAsync(CancellationToken cancellationToken = default)
    {
        var ventas = await _unitOfWork.Ventas.ObtenerTodasAsync(cancellationToken);
        return ventas.Select(MapearVentaDto);
    }

    public async Task<VentaDto?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var venta = await _unitOfWork.Ventas.ObtenerPorIdAsync(id, cancellationToken);
        return venta == null ? null : MapearVentaDto(venta);
    }

    public async Task<VentaDto?> ObtenerPorNumeroAsync(string numeroVenta, CancellationToken cancellationToken = default)
    {
        var venta = await _unitOfWork.Ventas.ObtenerPorNumeroAsync(numeroVenta, cancellationToken);
        return venta == null ? null : MapearVentaDto(venta);
    }

    public async Task<IEnumerable<VentaDto>> ObtenerPorClienteAsync(int clienteId, CancellationToken cancellationToken = default)
    {
        var ventas = await _unitOfWork.Ventas.ObtenerPorClienteAsync(clienteId, cancellationToken);
        return ventas.Select(MapearVentaDto);
    }

    public async Task<VentaDto> RegistrarVentaAsync(CrearVentaDto dto, CancellationToken cancellationToken = default)
    {
        var cliente = await _unitOfWork.Clientes.ObtenerPorIdAsync(dto.ClienteId, cancellationToken);
        if (cliente == null)
        {
            throw new KeyNotFoundException($"El cliente con ID {dto.ClienteId} no existe.");
        }

        if (dto.Items == null || dto.Items.Count == 0)
        {
            throw new ArgumentException("La venta debe contener al menos un producto.", nameof(dto));
        }

        var venta = new Venta
        {
            ClienteId = cliente.Id,
            Cliente = cliente,
            Fecha = DateTime.UtcNow,
            NumeroVenta = $"FAC-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}"
        };

        foreach (var item in dto.Items)
        {
            var producto = await _unitOfWork.Productos.ObtenerPorIdAsync(item.ProductoId, cancellationToken);
            if (producto == null)
            {
                throw new KeyNotFoundException($"El producto con ID {item.ProductoId} no existe.");
            }

            if (!producto.DescontarStock(item.Cantidad))
            {
                throw new InvalidOperationException(
                    $"Stock insuficiente para el producto '{producto.Nombre}'. Disponible: {producto.Stock}, Solicitado: {item.Cantidad}.");
            }

            var detalle = new DetalleVenta
            {
                ProductoId = producto.Id,
                Producto = producto,
                Cantidad = item.Cantidad,
                PrecioUnitario = producto.Precio,
                Subtotal = Math.Round(item.Cantidad * producto.Precio, 2)
            };

            venta.Detalles.Add(detalle);
            await _unitOfWork.Productos.ActualizarAsync(producto, cancellationToken);
        }

        venta.CalcularTotales();

        await _unitOfWork.Ventas.AgregarAsync(venta, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return MapearVentaDto(venta);
    }

    private static VentaDto MapearVentaDto(Venta venta)
    {
        return new VentaDto
        {
            Id = venta.Id,
            NumeroVenta = venta.NumeroVenta,
            Fecha = venta.Fecha,
            ClienteId = venta.ClienteId,
            NombreCliente = venta.Cliente?.Nombre ?? string.Empty,
            DocumentoCliente = venta.Cliente?.Documento ?? string.Empty,
            Subtotal = venta.Subtotal,
            Iva = venta.Iva,
            Total = venta.Total,
            Detalles = venta.Detalles.Select(d => new DetalleVentaDto
            {
                Id = d.Id,
                ProductoId = d.ProductoId,
                NombreProducto = d.Producto?.Nombre ?? string.Empty,
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario,
                Subtotal = d.Subtotal
            }).ToList()
        };
    }
}
