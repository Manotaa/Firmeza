using Firmeza.Domain.Entities;
using Xunit;

namespace Firmeza.Tests;

public class DomainTests
{
    [Fact]
    public void Producto_DescontarStock_DeberiaDisminuirStock_CuandoStockEsSuficiente()
    {
        // Arrange
        var producto = new Producto { Nombre = "Cemento Gris", Precio = 28000m, Stock = 50 };

        // Act
        var resultado = producto.DescontarStock(20);

        // Assert
        Assert.True(resultado);
        Assert.Equal(30, producto.Stock);
    }

    [Fact]
    public void Producto_DescontarStock_DeberiaRetornarFalse_CuandoStockEsInsuficiente()
    {
        // Arrange
        var producto = new Producto { Nombre = "Varilla 1/2", Precio = 42000m, Stock = 5 };

        // Act
        var resultado = producto.DescontarStock(10);

        // Assert
        Assert.False(resultado);
        Assert.Equal(5, producto.Stock);
    }

    [Fact]
    public void Producto_DescontarStock_DeberiaLanzarExcepcion_CuandoCantidadEsMenorOIgualACero()
    {
        // Arrange
        var producto = new Producto { Nombre = "Arena", Precio = 8000m, Stock = 100 };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => producto.DescontarStock(0));
        Assert.Throws<ArgumentException>(() => producto.DescontarStock(-5));
    }

    [Fact]
    public void Producto_AumentarStock_DeberiaIncrementarStock()
    {
        // Arrange
        var producto = new Producto { Nombre = "Ladrillo", Precio = 1200m, Stock = 100 };

        // Act
        producto.AumentarStock(50);

        // Assert
        Assert.Equal(150, producto.Stock);
    }

    [Fact]
    public void Venta_CalcularTotales_DeberiaCalcularSubtotalIvaYTotalCorrectamente()
    {
        // Arrange
        var venta = new Venta
        {
            NumeroVenta = "FAC-20261009-TEST",
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

        // Act (Tasa IVA por defecto 19%)
        venta.CalcularTotales();

        // Assert
        // Subtotal = 100000 + 20000 = 120000
        // IVA 19% = 120000 * 0.19 = 22800
        // Total = 120000 + 22800 = 142800
        Assert.Equal(120000m, venta.Subtotal);
        Assert.Equal(22800m, venta.Iva);
        Assert.Equal(142800m, venta.Total);
    }
}
