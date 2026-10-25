using Firmeza.Application.DTOs;
using Firmeza.Application.Interfaces;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Interfaces;

namespace Firmeza.Application.Services;

public class ProductoService : IProductoService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductoService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ProductoDto>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        var productos = await _unitOfWork.Productos.ObtenerTodosAsync(cancellationToken);
        return productos.Select(p => new ProductoDto
        {
            Id = p.Id,
            Nombre = p.Nombre,
            Precio = p.Precio,
            Stock = p.Stock
        });
    }

    public async Task<ProductoDto?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var p = await _unitOfWork.Productos.ObtenerPorIdAsync(id, cancellationToken);
        if (p == null) return null;

        return new ProductoDto
        {
            Id = p.Id,
            Nombre = p.Nombre,
            Precio = p.Precio,
            Stock = p.Stock
        };
    }

    public async Task<IEnumerable<ProductoDto>> BuscarAsync(string query, CancellationToken cancellationToken = default)
    {
        var productos = await _unitOfWork.Productos.BuscarPorNombreAsync(query, cancellationToken);
        return productos.Select(p => new ProductoDto
        {
            Id = p.Id,
            Nombre = p.Nombre,
            Precio = p.Precio,
            Stock = p.Stock
        });
    }

    public async Task<ProductoDto> CrearAsync(CrearProductoDto dto, CancellationToken cancellationToken = default)
    {
        var producto = new Producto
        {
            Nombre = dto.Nombre.Trim(),
            Precio = dto.Precio,
            Stock = dto.Stock
        };

        await _unitOfWork.Productos.AgregarAsync(producto, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return new ProductoDto
        {
            Id = producto.Id,
            Nombre = producto.Nombre,
            Precio = producto.Precio,
            Stock = producto.Stock
        };
    }

    public async Task<ProductoDto?> ActualizarAsync(int id, ActualizarProductoDto dto, CancellationToken cancellationToken = default)
    {
        var producto = await _unitOfWork.Productos.ObtenerPorIdAsync(id, cancellationToken);
        if (producto == null) return null;

        producto.Nombre = dto.Nombre.Trim();
        producto.Precio = dto.Precio;
        producto.Stock = dto.Stock;

        await _unitOfWork.Productos.ActualizarAsync(producto, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return new ProductoDto
        {
            Id = producto.Id,
            Nombre = producto.Nombre,
            Precio = producto.Precio,
            Stock = producto.Stock
        };
    }

    public async Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default)
    {
        var existe = await _unitOfWork.Productos.ExisteAsync(id, cancellationToken);
        if (!existe) return false;

        await _unitOfWork.Productos.EliminarAsync(id, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return true;
    }
}
