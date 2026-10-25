using Firmeza.Application.DTOs;
using Firmeza.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.Api.Controllers;

[ApiController]
[Route("api/ventas")]
[Route("api/sales")]
[Authorize]
public class VentasController : ControllerBase
{
    private readonly IVentaService _ventaService;
    private readonly IPdfReciboService _pdfService;
    private readonly IWebHostEnvironment _env;

    public VentasController(
        IVentaService ventaService,
        IPdfReciboService pdfService,
        IWebHostEnvironment env)
    {
        _ventaService = ventaService;
        _pdfService = pdfService;
        _env = env;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<VentaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var ventas = await _ventaService.ObtenerTodasAsync(cancellationToken);
        return Ok(ventas);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(VentaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var venta = await _ventaService.ObtenerPorIdAsync(id, cancellationToken);
        if (venta == null)
        {
            return NotFound(new { mensaje = $"La venta con ID {id} no fue encontrada." });
        }

        return Ok(venta);
    }

    [HttpGet("cliente/{clienteId:int}")]
    [ProducesResponseType(typeof(IEnumerable<VentaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCliente(int clienteId, CancellationToken cancellationToken)
    {
        var ventas = await _ventaService.ObtenerPorClienteAsync(clienteId, cancellationToken);
        return Ok(ventas);
    }

    [HttpPost]
    [ProducesResponseType(typeof(VentaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CrearVentaDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var ventaRegistrada = await _ventaService.RegistrarVentaAsync(dto, cancellationToken);

            // Generar y almacenar el recibo PDF automáticamente (Requisito Semana 2)
            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            await _pdfService.GuardarReciboPdfAsync(ventaRegistrada, webRoot, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = ventaRegistrada.Id }, ventaRegistrada);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpGet("{id:int}/recibo")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DescargarRecibo(int id, CancellationToken cancellationToken)
    {
        var venta = await _ventaService.ObtenerPorIdAsync(id, cancellationToken);
        if (venta == null)
        {
            return NotFound(new { mensaje = $"La venta con ID {id} no existe." });
        }

        var pdfBytes = await _pdfService.GenerarReciboPdfAsync(venta, cancellationToken);
        return File(pdfBytes, "application/pdf", $"Recibo-{venta.NumeroVenta}.pdf");
    }
}
