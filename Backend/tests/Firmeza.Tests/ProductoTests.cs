using Firmeza.Domain.Entities;
using Xunit;

namespace Firmeza.Tests;

public class ProductoTests
{
    [Fact]
    public void DescontarStock_ConStockSuficiente_DebeReducirStockYRetornarTrue()
    {
        // Arrange
        var producto = new Producto { Id = 1, Nombre = "Cemento", Stock = 20, Precio = 25000m };

        // Act
        var resultado = producto.DescontarStock(5);

        // Assert
        Assert.True(resultado);
        Assert.Equal(15, producto.Stock);
    }

    [Fact]
    public void DescontarStock_ConStockInsuficiente_DebeMantenerStockYRetornarFalse()
    {
        // Arrange
        var producto = new Producto { Id = 2, Nombre = "Varilla", Stock = 3, Precio = 40000m };

        // Act
        var resultado = producto.DescontarStock(10);

        // Assert
        Assert.False(resultado);
        Assert.Equal(3, producto.Stock);
    }

    [Fact]
    public void DescontarStock_ConCantidadCeroONegativa_DebeLanzarArgumentException()
    {
        // Arrange
        var producto = new Producto { Id = 3, Nombre = "Pintura", Stock = 10, Precio = 50000m };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => producto.DescontarStock(0));
        Assert.Throws<ArgumentException>(() => producto.DescontarStock(-5));
    }

    [Fact]
    public void AumentarStock_ConCantidadValida_DebeIncrementarStock()
    {
        // Arrange
        var producto = new Producto { Id = 4, Nombre = "Ladrillo", Stock = 100, Precio = 1200m };

        // Act
        producto.AumentarStock(50);

        // Assert
        Assert.Equal(150, producto.Stock);
    }
}
