namespace Firmeza.Domain.Entities;

public class Cliente
{
    public int Id { get; set; }
    public string Documento { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
}
