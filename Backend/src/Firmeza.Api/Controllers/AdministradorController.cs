using Firmeza.Application.Interfaces;
using Firmeza.Domain.Interfaces;
using Firmeza.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = Roles.Admin)]
public class AdministradorController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IExcelService _excelService;
    private readonly UserManager<UsuarioFirmeza> _userManager;

    public AdministradorController(
        IUnitOfWork unitOfWork,
        IExcelService excelService,
        UserManager<UsuarioFirmeza> userManager)
    {
        _unitOfWork = unitOfWork;
        _excelService = excelService;
        _userManager = userManager;
    }

    [HttpGet("dashboard")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboard(CancellationToken cancellationToken)
    {
        var totalProductos = (await _unitOfWork.Productos.ObtenerTodosAsync(cancellationToken)).Count();
        var totalClientes = (await _unitOfWork.Clientes.ObtenerTodosAsync(cancellationToken)).Count();
        var ventas = await _unitOfWork.Ventas.ObtenerTodasAsync(cancellationToken);
        var totalVentas = ventas.Count();
        var montoTotalVentas = ventas.Sum(v => v.Total);
        var totalUsuarios = await _userManager.Users.CountAsync(cancellationToken);

        return Ok(new
        {
            totalProductos,
            totalClientes,
            totalVentas,
            montoTotalVentas,
            totalUsuarios
        });
    }

    [HttpGet("usuarios")]
    [HttpGet("users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsuarios(CancellationToken cancellationToken)
    {
        var users = await _userManager.Users
            .OrderByDescending(u => u.FechaRegistro)
            .ToListAsync(cancellationToken);

        var lista = new List<object>();
        foreach (var u in users)
        {
            var roles = await _userManager.GetRolesAsync(u);
            lista.Add(new
            {
                u.Id,
                u.UserName,
                u.Email,
                u.NombreCompleto,
                Rol = roles.FirstOrDefault() ?? Roles.Cliente,
                u.FechaRegistro
            });
        }

        return Ok(lista);
    }

    [HttpGet("exportar-productos")]
    [HttpGet("products.xlsx")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportarProductos(CancellationToken cancellationToken)
    {
        var bytes = await _excelService.ExportarProductosAsync(cancellationToken);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "productos.xlsx");
    }

    [HttpGet("exportar-ventas")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportarVentas(CancellationToken cancellationToken)
    {
        var bytes = await _excelService.ExportarVentasAsync(cancellationToken);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ventas.xlsx");
    }

    [HttpPost("importar-excel")]
    [ProducesResponseType(typeof(ResultadoImportacionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ImportarExcel(IFormFile archivo, CancellationToken cancellationToken)
    {
        if (archivo == null || archivo.Length == 0)
        {
            return BadRequest(new { mensaje = "Por favor selecciona un archivo Excel (.xlsx) válido." });
        }

        if (!archivo.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { mensaje = "El archivo debe tener formato .xlsx de Excel." });
        }

        using var stream = archivo.OpenReadStream();
        var resultado = await _excelService.ImportarDatosAsync(stream, cancellationToken);

        return Ok(resultado);
    }
}
