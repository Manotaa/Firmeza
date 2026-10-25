using Firmeza.Application.Interfaces;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Interfaces;
using OfficeOpenXml;

namespace Firmeza.Infrastructure.Services;

public class ExcelService : IExcelService
{
    private readonly IUnitOfWork _unitOfWork;

    public ExcelService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public async Task<ResultadoImportacionDto> ImportarDatosAsync(Stream excelStream, CancellationToken cancellationToken = default)
    {
        var resultado = new ResultadoImportacionDto();

        using var package = new ExcelPackage(excelStream);
        var worksheet = package.Workbook.Worksheets.FirstOrDefault();

        if (worksheet == null || worksheet.Dimension == null)
        {
            resultado.Errores.Add("El archivo Excel está vacío o no contiene hojas válidas.");
            return resultado;
        }

        var rowCount = worksheet.Dimension.Rows;
        var colCount = worksheet.Dimension.Columns;

        if (rowCount < 2)
        {
            resultado.Errores.Add("El archivo no contiene filas de datos para procesar.");
            return resultado;
        }

        // Leer encabezados para mapeo flexible
        var headers = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (int col = 1; col <= colCount; col++)
        {
            var headerValue = worksheet.Cells[1, col].Text.Trim();
            if (!string.IsNullOrEmpty(headerValue))
            {
                headers[headerValue] = col;
            }
        }

        for (int row = 2; row <= rowCount; row++)
        {
            resultado.RegistrosProcesados++;
            try
            {
                // Procesar Producto si hay columnas de producto
                var colProducto = ObtenerColumna(headers, "NombreProducto", "Producto", "Nombre");
                var colPrecio = ObtenerColumna(headers, "PrecioProducto", "Precio");
                var colStock = ObtenerColumna(headers, "StockProducto", "Stock");

                if (colProducto.HasValue && !string.IsNullOrWhiteSpace(worksheet.Cells[row, colProducto.Value].Text))
                {
                    var nombre = worksheet.Cells[row, colProducto.Value].Text.Trim();
                    decimal precio = 0;
                    int stock = 0;

                    if (colPrecio.HasValue)
                    {
                        var precioText = worksheet.Cells[row, colPrecio.Value].Text.Trim();
                        decimal.TryParse(precioText, out precio);
                    }

                    if (colStock.HasValue)
                    {
                        var stockText = worksheet.Cells[row, colStock.Value].Text.Trim();
                        int.TryParse(stockText, out stock);
                    }

                    if (precio <= 0)
                    {
                        resultado.Errores.Add($"Fila {row}: El precio para el producto '{nombre}' debe ser mayor a 0.");
                    }
                    else
                    {
                        var productosExistentes = await _unitOfWork.Productos.BuscarPorNombreAsync(nombre, cancellationToken);
                        var producto = productosExistentes.FirstOrDefault(p => p.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase));

                        if (producto == null)
                        {
                            producto = new Producto
                            {
                                Nombre = nombre,
                                Precio = precio,
                                Stock = stock
                            };
                            await _unitOfWork.Productos.AgregarAsync(producto, cancellationToken);
                            resultado.ProductosImportados++;
                        }
                        else
                        {
                            producto.Precio = precio;
                            producto.Stock = stock;
                            await _unitOfWork.Productos.ActualizarAsync(producto, cancellationToken);
                            resultado.ProductosImportados++;
                        }
                    }
                }

                // Procesar Cliente si hay columnas de cliente
                var colClienteDoc = ObtenerColumna(headers, "DocumentoCliente", "Documento", "Cedula", "Nit");
                var colClienteNombre = ObtenerColumna(headers, "NombreCliente", "Cliente", "RazonSocial");

                if (colClienteDoc.HasValue && !string.IsNullOrWhiteSpace(worksheet.Cells[row, colClienteDoc.Value].Text))
                {
                    var doc = worksheet.Cells[row, colClienteDoc.Value].Text.Trim();
                    var nombreCliente = colClienteNombre.HasValue ? worksheet.Cells[row, colClienteNombre.Value].Text.Trim() : $"Cliente {doc}";

                    if (string.IsNullOrWhiteSpace(nombreCliente))
                    {
                        nombreCliente = $"Cliente {doc}";
                    }

                    var cliente = await _unitOfWork.Clientes.ObtenerPorDocumentoAsync(doc, cancellationToken);
                    if (cliente == null)
                    {
                        cliente = new Cliente
                        {
                            Documento = doc,
                            Nombre = nombreCliente
                        };
                        await _unitOfWork.Clientes.AgregarAsync(cliente, cancellationToken);
                        resultado.ClientesImportados++;
                    }
                    else
                    {
                        cliente.Nombre = nombreCliente;
                        await _unitOfWork.Clientes.ActualizarAsync(cliente, cancellationToken);
                        resultado.ClientesImportados++;
                    }
                }
            }
            catch (Exception ex)
            {
                resultado.Errores.Add($"Fila {row}: Error al procesar - {ex.Message}");
            }
        }

        await _unitOfWork.CommitAsync(cancellationToken);
        return resultado;
    }

    public async Task<byte[]> ExportarProductosAsync(CancellationToken cancellationToken = default)
    {
        var productos = await _unitOfWork.Productos.ObtenerTodosAsync(cancellationToken);

        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("Productos");

        // Encabezados
        ws.Cells[1, 1].Value = "ID";
        ws.Cells[1, 2].Value = "Nombre";
        ws.Cells[1, 3].Value = "Precio";
        ws.Cells[1, 4].Value = "Stock";

        using (var headerRange = ws.Cells[1, 1, 1, 4])
        {
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        }

        int row = 2;
        foreach (var p in productos)
        {
            ws.Cells[row, 1].Value = p.Id;
            ws.Cells[row, 2].Value = p.Nombre;
            ws.Cells[row, 3].Value = p.Precio;
            ws.Cells[row, 4].Value = p.Stock;
            row++;
        }

        ws.Cells[ws.Dimension.Address].AutoFitColumns();
        return await package.GetAsByteArrayAsync();
    }

    public async Task<byte[]> ExportarVentasAsync(CancellationToken cancellationToken = default)
    {
        var ventas = await _unitOfWork.Ventas.ObtenerTodasAsync(cancellationToken);

        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("Ventas");

        // Encabezados
        ws.Cells[1, 1].Value = "ID";
        ws.Cells[1, 2].Value = "N° Venta";
        ws.Cells[1, 3].Value = "Fecha";
        ws.Cells[1, 4].Value = "Cliente";
        ws.Cells[1, 5].Value = "Documento";
        ws.Cells[1, 6].Value = "Subtotal";
        ws.Cells[1, 7].Value = "IVA (19%)";
        ws.Cells[1, 8].Value = "Total";

        using (var headerRange = ws.Cells[1, 1, 1, 8])
        {
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightSteelBlue);
        }

        int row = 2;
        foreach (var v in ventas)
        {
            ws.Cells[row, 1].Value = v.Id;
            ws.Cells[row, 2].Value = v.NumeroVenta;
            ws.Cells[row, 3].Value = v.Fecha.ToString("yyyy-MM-dd HH:mm");
            ws.Cells[row, 4].Value = v.Cliente?.Nombre ?? "N/A";
            ws.Cells[row, 5].Value = v.Cliente?.Documento ?? "N/A";
            ws.Cells[row, 6].Value = v.Subtotal;
            ws.Cells[row, 7].Value = v.Iva;
            ws.Cells[row, 8].Value = v.Total;
            row++;
        }

        ws.Cells[ws.Dimension.Address].AutoFitColumns();
        return await package.GetAsByteArrayAsync();
    }

    private static int? ObtenerColumna(Dictionary<string, int> headers, params string[] nombresPosibles)
    {
        foreach (var nombre in nombresPosibles)
        {
            if (headers.TryGetValue(nombre, out var colIndex))
            {
                return colIndex;
            }
        }
        return null;
    }
}
