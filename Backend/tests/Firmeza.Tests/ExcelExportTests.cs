using OfficeOpenXml;
using Xunit;

namespace Firmeza.Tests;

public class ExcelExportTests
{
    [Fact]
    public void GenerarHojaExcel_ConEPPlus_DebeCrearEstructuraCorrecta()
    {
        // Arrange
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using var package = new ExcelPackage();

        // Act
        var worksheet = package.Workbook.Worksheets.Add("Productos");
        worksheet.Cells[1, 1].Value = "ID";
        worksheet.Cells[1, 2].Value = "Nombre";
        worksheet.Cells[1, 3].Value = "Precio";
        worksheet.Cells[1, 4].Value = "Stock";

        worksheet.Cells[2, 1].Value = 1;
        worksheet.Cells[2, 2].Value = "Cemento Gris 50kg";
        worksheet.Cells[2, 3].Value = 28500m;
        worksheet.Cells[2, 4].Value = 100;

        var bytes = package.GetAsByteArray();

        // Assert
        Assert.NotNull(bytes);
        Assert.True(bytes.Length > 0, "El archivo Excel generado debe contener bytes.");

        // Validar que se puede leer de vuelta
        using var stream = new MemoryStream(bytes);
        using var readPackage = new ExcelPackage(stream);
        var readSheet = readPackage.Workbook.Worksheets["Productos"];
        Assert.NotNull(readSheet);
        Assert.Equal("Cemento Gris 50kg", readSheet.Cells[2, 2].Text);
        Assert.Equal("28500", readSheet.Cells[2, 3].Text);
    }
}
