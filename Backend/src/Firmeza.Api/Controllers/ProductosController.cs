using Firmeza.Application.DTOs;
using Firmeza.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.Api.Controllers;

[ApiController]
[Route("api/productos")]
[Route("api/products")]
public class ProductosController : ControllerBase
{
    private readonly IProductoService _productoService;

    public ProductosController(IProductoService productoService)
    {
        _productoService = productoService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] string? buscar, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var filtrados = await _productoService.BuscarAsync(buscar, cancellationToken);
            return Ok(filtrados);
        }

        var productos = await _productoService.ObtenerTodosAsync(cancellationToken);
        return Ok(productos);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var producto = await _productoService.ObtenerPorIdAsync(id, cancellationToken);
        if (producto == null)
        {
            return NotFound(new { mensaje = $"El producto con ID {id} no fue encontrado." });
        }

        return Ok(producto);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ProductoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CrearProductoDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var creado = await _productoService.CrearAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ProductoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] ActualizarProductoDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var actualizado = await _productoService.ActualizarAsync(id, dto, cancellationToken);
        if (actualizado == null)
        {
            return NotFound(new { mensaje = $"El producto con ID {id} no existe." });
        }

        return Ok(actualizado);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var eliminado = await _productoService.EliminarAsync(id, cancellationToken);
        if (!eliminado)
        {
            return NotFound(new { mensaje = $"El producto con ID {id} no existe." });
        }

        return NoContent();
    }
}
