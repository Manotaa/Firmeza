namespace Firmeza.Domain.Interfaces;

public interface IUnitOfWork
{
    IProductoRepository Productos { get; }
    IClienteRepository Clientes { get; }
    IVentaRepository Ventas { get; }

    Task<int> CommitAsync(CancellationToken cancellationToken = default);
}
