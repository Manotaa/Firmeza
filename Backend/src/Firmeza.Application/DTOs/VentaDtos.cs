using System.ComponentModel.DataAnnotations;

namespace Firmeza.Application.DTOs;

public class DetalleVentaDto
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
}

public class VentaDto
{
    public int Id { get; set; }
    public string NumeroVenta { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public int ClienteId { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public string DocumentoCliente { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
    public decimal Iva { get; set; }
    public decimal Total { get; set; }
    public List<DetalleVentaDto> Detalles { get; set; } = new();
}

public class CrearDetalleVentaDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "El ID del producto debe ser válido.")]
    public int ProductoId { get; set; }

    [Required]
    [Range(1, 10000, ErrorMessage = "La cantidad debe ser mayor a 0.")]
    public int Cantidad { get; set; }
}

public class CrearVentaDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "El ID del cliente debe ser válido.")]
    public int ClienteId { get; set; }

    [Required(ErrorMessage = "La venta debe contener al menos un producto.")]
    [MinLength(1, ErrorMessage = "La venta debe contener al menos un producto.")]
    public List<CrearDetalleVentaDto> Items { get; set; } = new();
}
