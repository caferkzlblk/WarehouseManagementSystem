using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.IO;
using System.Text;
using WarehouseManagementSystem.Data;

[Route("api/[controller]")]
[ApiController]
public class ReportsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConverter _converter;

    public ReportsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("Orders/Excel")]
    public async Task<IActionResult> ExportOrdersToExcel([FromQuery] int? statusID, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        var ordersQuery = _context.Orders
            .Include(o => o.Status)
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product)
            .AsQueryable();

        // Filtreler
        if (statusID.HasValue)
            ordersQuery = ordersQuery.Where(o => o.StatusID == statusID.Value);
        if (startDate.HasValue)
            ordersQuery = ordersQuery.Where(o => o.OrderDate >= startDate.Value);
        if (endDate.HasValue)
            ordersQuery = ordersQuery.Where(o => o.OrderDate <= endDate.Value);

        var orders = await ordersQuery.ToListAsync();

        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("OrdersReport");
            worksheet.Cells[1, 1].Value = "Sipariş Tarihi";
            worksheet.Cells[1, 2].Value = "Sipariş No";
            worksheet.Cells[1, 3].Value = "Durum";
            worksheet.Cells[1, 4].Value = "Ürün";
            worksheet.Cells[1, 5].Value = "Miktar";
            worksheet.Cells[1, 6].Value = "Tutar";

            int row = 2;
            foreach (var order in orders)
            {
                foreach (var detail in order.OrderDetails)
                {
                    worksheet.Cells[row, 1].Value = order.OrderDate.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 2].Value = order.OrderNumberValue;
                    worksheet.Cells[row, 3].Value = order.Status?.StatusName;
                    worksheet.Cells[row, 4].Value = detail.Product?.ProductName;
                    worksheet.Cells[row, 5].Value = detail.Quantity;
                    worksheet.Cells[row, 6].Value = detail.Price;
                    row++;
                }
            }
            worksheet.Cells.AutoFitColumns();

            var excelBytes = package.GetAsByteArray();
            var fileName = $"SiparisRaporu_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
    [HttpGet("Orders/Pdf")]
    public async Task<IActionResult> ExportOrdersToPdf([FromQuery] int? statusID, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var ordersQuery = _context.Orders
            .Include(o => o.Status)
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product)
            .AsQueryable();

        if (statusID.HasValue)
            ordersQuery = ordersQuery.Where(o => o.StatusID == statusID.Value);
        if (startDate.HasValue)
            ordersQuery = ordersQuery.Where(o => o.OrderDate >= startDate.Value);
        if (endDate.HasValue)
            ordersQuery = ordersQuery.Where(o => o.OrderDate <= endDate.Value);

        var orders = await ordersQuery.ToListAsync();

        // 1. Dinamik HTML şablonu oluştur (istediğine göre özelleştir!)
        var html = new StringBuilder();
        html.AppendLine("<h2>Sipariş Raporu</h2>");
        html.AppendLine("<table border='1' cellpadding='5' cellspacing='0' style='width:100%;border-collapse:collapse;'>");
        html.AppendLine("<thead><tr><th>Tarih</th><th>Sipariş No</th><th>Durum</th><th>Ürün</th><th>Miktar</th><th>Fiyat</th></tr></thead>");
        html.AppendLine("<tbody>");

        foreach (var order in orders)
        {
            foreach (var detail in order.OrderDetails)
            {
                html.AppendLine("<tr>");
                html.AppendLine($"<td>{order.OrderDate:yyyy-MM-dd}</td>");
                html.AppendLine($"<td>{order.OrderNumberValue}</td>");
                html.AppendLine($"<td>{order.Status?.StatusName}</td>");
                html.AppendLine($"<td>{detail.Product?.ProductName}</td>");
                html.AppendLine($"<td>{detail.Quantity}</td>");
                html.AppendLine($"<td>{detail.Price}</td>");
                html.AppendLine("</tr>");
            }
        }

        html.AppendLine("</tbody></table>");

        // 2. HTML’den PDF oluştur
        var pdfDoc = new HtmlToPdfDocument()
        {
            GlobalSettings = {
                PaperSize = PaperKind.A4,
                Orientation = Orientation.Landscape,
                Margins = new MarginSettings { Top = 15, Bottom = 15 }
            },
            Objects = {
                new ObjectSettings()
                {
                    HtmlContent = html.ToString(),
                    WebSettings = { DefaultEncoding = "utf-8" }
                }
            }
        };

        var pdfBytes = _converter.Convert(pdfDoc);

        var fileName = $"SiparisRaporu_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
        return File(pdfBytes, "application/pdf", fileName);
    }
}
