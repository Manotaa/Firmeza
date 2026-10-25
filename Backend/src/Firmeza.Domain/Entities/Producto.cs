namespace Firmeza.Domain.Entities;

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Stock { get; set; }

    // Navigation properties
    public ICollection<DetalleVenta> DetallesVenta { get; set; } = new List<DetalleVenta>();

    public bool DescontarStock(int cantidad)
    {
        if (cantidad <= 0)
            throw new ArgumentException("La cantidad a descontar debe ser mayor a cero.", nameof(cantidad));

        if (Stock < cantidad)
            return false;

        Stock -= cantidad;
        return true;
    }

    public void AumentarStock(int cantidad)
    {
        if (cantidad <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero.", nameof(cantidad));

        Stock += cantidad;
    }
}
