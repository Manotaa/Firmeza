using Firmeza.Application.DTOs;
using Firmeza.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.Api.Controllers;

[ApiController]
[Route("api/clientes")]
[Route("api/clients")]
[Authorize(Roles = "Admin")]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClienteDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] string? buscar, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var filtrados = await _clienteService.BuscarAsync(buscar, cancellationToken);
            return Ok(filtrados);
        }

        var clientes = await _clienteService.ObtenerTodosAsync(cancellationToken);
        return Ok(clientes);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var cliente = await _clienteService.ObtenerPorIdAsync(id, cancellationToken);
        if (cliente == null)
        {
            return NotFound(new { mensaje = $"El cliente con ID {id} no fue encontrado." });
        }

        return Ok(cliente);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CrearClienteDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var creado = await _clienteService.CrearAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] ActualizarClienteDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var actualizado = await _clienteService.ActualizarAsync(id, dto, cancellationToken);
            if (actualizado == null)
            {
                return NotFound(new { mensaje = $"El cliente con ID {id} no existe." });
            }

            return Ok(actualizado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var eliminado = await _clienteService.EliminarAsync(id, cancellationToken);
        if (!eliminado)
        {
            return NotFound(new { mensaje = $"El cliente con ID {id} no existe." });
        }

        return NoContent();
    }
}
