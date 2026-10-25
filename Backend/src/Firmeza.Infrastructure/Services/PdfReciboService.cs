using Firmeza.Application.DTOs;
using Firmeza.Application.Interfaces;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace Firmeza.Infrastructure.Services;

public class PdfReciboService : IPdfReciboService
{
    public Task<byte[]> GenerarReciboPdfAsync(VentaDto venta, CancellationToken cancellationToken = default)
    {
        using var memoryStream = new MemoryStream();
        using var writer = new PdfWriter(memoryStream);
        using var pdf = new PdfDocument(writer);
        using var document = new Document(pdf, PageSize.A4);
        document.SetMargins(36, 36, 36, 36);

        var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
        var italicFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_OBLIQUE);
        var regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

        document.SetFont(regularFont);

        // Colores de la marca Firmeza
        var primaryColor = new DeviceRgb(30, 58, 138);     // Azul corporativo (#1E3A8A)
        var secondaryColor = new DeviceRgb(243, 244, 246); // Gris suave

        // Encabezado
        var headerTable = new Table(UnitValue.CreatePercentArray(new float[] { 60, 40 })).UseAllAvailableWidth();
        headerTable.SetBorder(Border.NO_BORDER);

        var titleCell = new Cell()
            .Add(new Paragraph("FIRMEZA").SetFontSize(24).SetFont(boldFont).SetFontColor(primaryColor))
            .Add(new Paragraph("Materiales y Suministros para la Construcción").SetFontSize(10).SetFontColor(ColorConstants.GRAY))
            .SetBorder(Border.NO_BORDER);

        var receiptMetaCell = new Cell()
            .Add(new Paragraph("COMPROBANTE DE VENTA").SetFontSize(14).SetFont(boldFont).SetTextAlignment(TextAlignment.RIGHT).SetFontColor(primaryColor))
            .Add(new Paragraph($"N°: {venta.NumeroVenta}").SetFontSize(11).SetFont(boldFont).SetTextAlignment(TextAlignment.RIGHT))
            .Add(new Paragraph($"Fecha: {venta.Fecha:yyyy-MM-dd HH:mm}").SetFontSize(9).SetTextAlignment(TextAlignment.RIGHT))
            .SetBorder(Border.NO_BORDER);

        headerTable.AddCell(titleCell);
        headerTable.AddCell(receiptMetaCell);
        document.Add(headerTable);

        document.Add(new Paragraph("\n"));

        // Datos del Cliente
        var clientTable = new Table(UnitValue.CreatePercentArray(new float[] { 50, 50 })).UseAllAvailableWidth();
        clientTable.SetBackgroundColor(secondaryColor);
        clientTable.SetBorder(new SolidBorder(ColorConstants.LIGHT_GRAY, 1));
        clientTable.SetPadding(8);

        clientTable.AddCell(new Cell().Add(new Paragraph("DATOS DEL CLIENTE").SetFontSize(10).SetFont(boldFont).SetFontColor(primaryColor)).SetBorder(Border.NO_BORDER));
        clientTable.AddCell(new Cell().SetBorder(Border.NO_BORDER));

        clientTable.AddCell(new Cell().Add(new Paragraph($"Nombre / Razón Social: {venta.NombreCliente}").SetFontSize(10)).SetBorder(Border.NO_BORDER));
        clientTable.AddCell(new Cell().Add(new Paragraph($"Documento / NIT: {venta.DocumentoCliente}").SetFontSize(10)).SetBorder(Border.NO_BORDER));

        document.Add(clientTable);
        document.Add(new Paragraph("\n"));

        // Tabla de Productos
        var itemsTable = new Table(UnitValue.CreatePercentArray(new float[] { 45, 15, 20, 20 })).UseAllAvailableWidth();
        itemsTable.SetBorder(Border.NO_BORDER);

        // Cabecera de la tabla
        string[] headers = { "Producto", "Cantidad", "Precio Unitario", "Subtotal" };
        foreach (var header in headers)
        {
            var cell = new Cell()
                .Add(new Paragraph(header).SetFontSize(10).SetFont(boldFont).SetFontColor(ColorConstants.WHITE))
                .SetBackgroundColor(primaryColor)
                .SetTextAlignment(header == "Producto" ? TextAlignment.LEFT : TextAlignment.RIGHT)
                .SetPadding(6);
            itemsTable.AddHeaderCell(cell);
        }

        // Filas de productos
        bool isAlt = false;
        foreach (var item in venta.Detalles)
        {
            var rowBg = isAlt ? secondaryColor : ColorConstants.WHITE;

            itemsTable.AddCell(new Cell().Add(new Paragraph(item.NombreProducto).SetFontSize(9)).SetBackgroundColor(rowBg).SetPadding(6).SetBorder(new SolidBorder(ColorConstants.LIGHT_GRAY, 0.5f)));
            itemsTable.AddCell(new Cell().Add(new Paragraph(item.Cantidad.ToString()).SetFontSize(9)).SetTextAlignment(TextAlignment.RIGHT).SetBackgroundColor(rowBg).SetPadding(6).SetBorder(new SolidBorder(ColorConstants.LIGHT_GRAY, 0.5f)));
            itemsTable.AddCell(new Cell().Add(new Paragraph($"${item.PrecioUnitario:N2}").SetFontSize(9)).SetTextAlignment(TextAlignment.RIGHT).SetBackgroundColor(rowBg).SetPadding(6).SetBorder(new SolidBorder(ColorConstants.LIGHT_GRAY, 0.5f)));
            itemsTable.AddCell(new Cell().Add(new Paragraph($"${item.Subtotal:N2}").SetFontSize(9)).SetTextAlignment(TextAlignment.RIGHT).SetBackgroundColor(rowBg).SetPadding(6).SetBorder(new SolidBorder(ColorConstants.LIGHT_GRAY, 0.5f)));

            isAlt = !isAlt;
        }

        document.Add(itemsTable);
        document.Add(new Paragraph("\n"));

        // Totales
        var totalsTable = new Table(UnitValue.CreatePercentArray(new float[] { 60, 40 })).UseAllAvailableWidth();
        totalsTable.SetBorder(Border.NO_BORDER);

        var noteCell = new Cell()
            .Add(new Paragraph("Gracias por su preferencia.").SetFontSize(9).SetFont(italicFont))
            .Add(new Paragraph("Comprobante generado electrónicamente por Sistema Firmeza.").SetFontSize(8).SetFontColor(ColorConstants.GRAY))
            .SetBorder(Border.NO_BORDER);

        var totalsCell = new Cell().SetBorder(Border.NO_BORDER);
        var innerTotals = new Table(UnitValue.CreatePercentArray(new float[] { 50, 50 })).UseAllAvailableWidth();
        innerTotals.SetBorder(Border.NO_BORDER);

        innerTotals.AddCell(new Cell().Add(new Paragraph("Subtotal:").SetFontSize(10)).SetBorder(Border.NO_BORDER));
        innerTotals.AddCell(new Cell().Add(new Paragraph($"${venta.Subtotal:N2}").SetFontSize(10).SetTextAlignment(TextAlignment.RIGHT)).SetBorder(Border.NO_BORDER));

        innerTotals.AddCell(new Cell().Add(new Paragraph("IVA (19%):").SetFontSize(10)).SetBorder(Border.NO_BORDER));
        innerTotals.AddCell(new Cell().Add(new Paragraph($"${venta.Iva:N2}").SetFontSize(10).SetTextAlignment(TextAlignment.RIGHT)).SetBorder(Border.NO_BORDER));

        innerTotals.AddCell(new Cell().Add(new Paragraph("TOTAL:").SetFontSize(12).SetFont(boldFont).SetFontColor(primaryColor)).SetBorder(new SolidBorder(primaryColor, 1)));
        innerTotals.AddCell(new Cell().Add(new Paragraph($"${venta.Total:N2}").SetFontSize(12).SetFont(boldFont).SetFontColor(primaryColor).SetTextAlignment(TextAlignment.RIGHT)).SetBorder(new SolidBorder(primaryColor, 1)));

        totalsCell.Add(innerTotals);

        totalsTable.AddCell(noteCell);
        totalsTable.AddCell(totalsCell);

        document.Add(totalsTable);

        document.Close();
        return Task.FromResult(memoryStream.ToArray());
    }

    public async Task<string> GuardarReciboPdfAsync(VentaDto venta, string webRootPath, CancellationToken cancellationToken = default)
    {
        var recibosDir = System.IO.Path.Combine(webRootPath, "recibos");
        if (!Directory.Exists(recibosDir))
        {
            Directory.CreateDirectory(recibosDir);
        }

        var fileName = $"{venta.NumeroVenta}.pdf";
        var fullPath = System.IO.Path.Combine(recibosDir, fileName);

        var bytes = await GenerarReciboPdfAsync(venta, cancellationToken);
        await File.WriteAllBytesAsync(fullPath, bytes, cancellationToken);

        return $"/recibos/{fileName}";
    }
}
