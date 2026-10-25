using Firmeza.Application.DTOs;
using Firmeza.Application.Interfaces;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Interfaces;

namespace Firmeza.Application.Services;

public class ClienteService : IClienteService
{
    private readonly IUnitOfWork _unitOfWork;

    public ClienteService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ClienteDto>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        var clientes = await _unitOfWork.Clientes.ObtenerTodosAsync(cancellationToken);
        return clientes.Select(c => new ClienteDto
        {
            Id = c.Id,
            Documento = c.Documento,
            Nombre = c.Nombre
        });
    }

    public async Task<IEnumerable<ClienteDto>> BuscarAsync(string query, CancellationToken cancellationToken = default)
    {
        var clientes = await _unitOfWork.Clientes.ObtenerTodosAsync(cancellationToken);
        var q = query.Trim().ToLower();
        return clientes
            .Where(c => c.Nombre.ToLower().Contains(q) || c.Documento.ToLower().Contains(q))
            .Select(c => new ClienteDto
            {
                Id = c.Id,
                Documento = c.Documento,
                Nombre = c.Nombre
            });
    }

    public async Task<ClienteDto?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var c = await _unitOfWork.Clientes.ObtenerPorIdAsync(id, cancellationToken);
        if (c == null) return null;

        return new ClienteDto
        {
            Id = c.Id,
            Documento = c.Documento,
            Nombre = c.Nombre
        };
    }

    public async Task<ClienteDto?> ObtenerPorDocumentoAsync(string documento, CancellationToken cancellationToken = default)
    {
        var c = await _unitOfWork.Clientes.ObtenerPorDocumentoAsync(documento, cancellationToken);
        if (c == null) return null;

        return new ClienteDto
        {
            Id = c.Id,
            Documento = c.Documento,
            Nombre = c.Nombre
        };
    }

    public async Task<ClienteDto> CrearAsync(CrearClienteDto dto, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Clientes.ExisteDocumentoAsync(dto.Documento.Trim(), cancellationToken))
        {
            throw new InvalidOperationException($"Ya existe un cliente con el documento {dto.Documento}.");
        }

        var cliente = new Cliente
        {
            Documento = dto.Documento.Trim(),
            Nombre = dto.Nombre.Trim()
        };

        await _unitOfWork.Clientes.AgregarAsync(cliente, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return new ClienteDto
        {
            Id = cliente.Id,
            Documento = cliente.Documento,
            Nombre = cliente.Nombre
        };
    }

    public async Task<ClienteDto?> ActualizarAsync(int id, ActualizarClienteDto dto, CancellationToken cancellationToken = default)
    {
        var cliente = await _unitOfWork.Clientes.ObtenerPorIdAsync(id, cancellationToken);
        if (cliente == null) return null;

        cliente.Documento = dto.Documento.Trim();
        cliente.Nombre = dto.Nombre.Trim();

        await _unitOfWork.Clientes.ActualizarAsync(cliente, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return new ClienteDto
        {
            Id = cliente.Id,
            Documento = cliente.Documento,
            Nombre = cliente.Nombre
        };
    }

    public async Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default)
    {
        var existe = await _unitOfWork.Clientes.ExisteAsync(id, cancellationToken);
        if (!existe) return false;

        await _unitOfWork.Clientes.EliminarAsync(id, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return true;
    }
}
