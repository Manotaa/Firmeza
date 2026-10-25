namespace Firmeza.Domain.Entities;

public class Venta
{
    public const decimal TasaIvaPorDefecto = 0.19m; // 19% IVA

    public int Id { get; set; }
    public string NumeroVenta { get; set; } = string.Empty;
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public int ClienteId { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Iva { get; set; }
    public decimal Total { get; set; }

    // Navigation properties
    public Cliente? Cliente { get; set; }
    public ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();

    public void CalcularTotales(decimal tasaIva = TasaIvaPorDefecto)
    {
        Subtotal = Math.Round(Detalles.Sum(d => d.Subtotal), 2);
        Iva = Math.Round(Subtotal * tasaIva, 2);
        Total = Subtotal + Iva;
    }
}
