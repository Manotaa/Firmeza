using Firmeza.Domain.Entities;
using Xunit;

namespace Firmeza.Tests;

public class VentaTests
{
    [Fact]
    public void CalcularTotales_ConDetallesValidos_DebeCalcularSubtotalIvaYTotalCorrectamente()
    {
        // Arrange
        var venta = new Venta
        {
            Id = 1,
            NumeroVenta = "FAC-2026-001",
            ClienteId = 1
        };

        venta.Detalles.Add(new DetalleVenta
        {
            ProductoId = 1,
            Cantidad = 2,
            PrecioUnitario = 50000m,
            Subtotal = 100000m
        });

        venta.Detalles.Add(new DetalleVenta
        {
            ProductoId = 2,
            Cantidad = 1,
            PrecioUnitario = 20000m,
            Subtotal = 20000m
        });

        // Act
        venta.CalcularTotales(0.19m); // 19% IVA

        // Assert
        Assert.Equal(120000m, venta.Subtotal);
        Assert.Equal(22800m, venta.Iva); // 120000 * 0.19 = 22800
        Assert.Equal(142800m, venta.Total); // 120000 + 22800 = 142800
    }

    [Fact]
    public void CalcularTotales_SinDetalles_DebeTenerTotalesEnCero()
    {
        // Arrange
        var venta = new Venta
        {
            Id = 2,
            NumeroVenta = "FAC-2026-002",
            ClienteId = 1
        };

        // Act
        venta.CalcularTotales();

        // Assert
        Assert.Equal(0m, venta.Subtotal);
        Assert.Equal(0m, venta.Iva);
        Assert.Equal(0m, venta.Total);
    }
}
