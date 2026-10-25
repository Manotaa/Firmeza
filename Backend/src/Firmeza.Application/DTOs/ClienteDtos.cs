using System.ComponentModel.DataAnnotations;

namespace Firmeza.Application.DTOs;

public class ClienteDto
{
    public int Id { get; set; }
    public string Documento { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
}

public class CrearClienteDto
{
    [Required(ErrorMessage = "El documento es obligatorio.")]
    [StringLength(20, MinimumLength = 5, ErrorMessage = "El documento debe tener entre 5 y 20 caracteres.")]
    public string Documento { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre del cliente es obligatorio.")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 150 caracteres.")]
    public string Nombre { get; set; } = string.Empty;
}

public class ActualizarClienteDto
{
    [Required(ErrorMessage = "El documento es obligatorio.")]
    [StringLength(20, MinimumLength = 5, ErrorMessage = "El documento debe tener entre 5 y 20 caracteres.")]
    public string Documento { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre del cliente es obligatorio.")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 150 caracteres.")]
    public string Nombre { get; set; } = string.Empty;
}
