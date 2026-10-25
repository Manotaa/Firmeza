using Firmeza.Application.DTOs;
using Firmeza.Infrastructure.Services;
using Xunit;

namespace Firmeza.Tests;

public class PdfReciboServiceTests
{
    [Fact]
    public async Task GenerarReciboPdfAsync_ConDatosDeVenta_DebeGenerarBytesValidosDePdf()
    {
        // Arrange
        var service = new PdfReciboService();
        var ventaDto = new VentaDto
        {
            Id = 10,
            NumeroVenta = "FAC-20261024-001",
            Fecha = DateTime.UtcNow,
            ClienteId = 1,
            NombreCliente = "Constructora Bolívar SAS",
            DocumentoCliente = "1012345678",
            Subtotal = 100000m,
            Iva = 19000m,
            Total = 119000m,
            Detalles = new List<DetalleVentaDto>
            {
                new()
                {
                    Id = 1,
                    ProductoId = 1,
                    NombreProducto = "Cemento Gris 50kg",
                    Cantidad = 2,
                    PrecioUnitario = 50000m,
                    Subtotal = 100000m
                }
            }
        };

        // Act
        var pdfBytes = await service.GenerarReciboPdfAsync(ventaDto);

        // Assert
        Assert.NotNull(pdfBytes);
        Assert.True(pdfBytes.Length > 0, "El PDF generado no debe estar vacío.");

        // Validar firma mágica del encabezado PDF: %PDF-
        var header = System.Text.Encoding.ASCII.GetString(pdfBytes.Take(4).ToArray());
        Assert.Equal("%PDF", header);
    }
}
