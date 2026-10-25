namespace Firmeza.Application.Interfaces;

public class ResultadoImportacionDto
{
    public int RegistrosProcesados { get; set; }
    public int ProductosImportados { get; set; }
    public int ClientesImportados { get; set; }
    public List<string> Errores { get; set; } = new();
    public bool Exitoso => Errores.Count == 0;
}

public interface IExcelService
{
    Task<ResultadoImportacionDto> ImportarDatosAsync(Stream excelStream, CancellationToken cancellationToken = default);
    Task<byte[]> ExportarProductosAsync(CancellationToken cancellationToken = default);
    Task<byte[]> ExportarVentasAsync(CancellationToken cancellationToken = default);
}
