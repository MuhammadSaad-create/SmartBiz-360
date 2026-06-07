using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp;
using SmartBiz360.Data;

namespace SmartBiz360.Services
{
    public class ReportService
    {
        private readonly AppDbContext _db;

        public ReportService(AppDbContext db)
        {
            _db = db;
        }

        // ── CSV Export ──────────────────────────────────────────
        public async Task<byte[]> ExportEmployeesCsvAsync()
        {
            var employees = await _db.Employees
                .Include(e => e.User)
                .ToListAsync();

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Employees");

            // Headers
            ws.Cell(1, 1).Value = "ID";
            ws.Cell(1, 2).Value = "Full Name";
            ws.Cell(1, 3).Value = "Email";
            ws.Cell(1, 4).Value = "Department";
            ws.Cell(1, 5).Value = "Position";
            ws.Cell(1, 6).Value = "Status";
            ws.Cell(1, 7).Value = "Join Date";

            // Style headers
            var headerRow = ws.Range("A1:G1");
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor =
                XLColor.FromHtml("#1e3a8a");
            headerRow.Style.Font.FontColor = XLColor.White;

            // Data
            int row = 2;
            foreach (var emp in employees)
            {
                ws.Cell(row, 1).Value = emp.Id;
                ws.Cell(row, 2).Value = emp.User.FullName;
                ws.Cell(row, 3).Value = emp.User.Email;
                ws.Cell(row, 4).Value = emp.Department;
                ws.Cell(row, 5).Value = emp.Position;
                ws.Cell(row, 6).Value = emp.EmploymentStatus;
                ws.Cell(row, 7).Value = emp.JoinDate
                    .ToString("yyyy-MM-dd");
                row++;
            }

            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }

        public async Task<byte[]> ExportCustomersCsvAsync()
        {
            var customers = await _db.Customers
                .Include(c => c.AssignedRep)
                .ToListAsync();

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Customers");

            ws.Cell(1, 1).Value = "ID";
            ws.Cell(1, 2).Value = "Company";
            ws.Cell(1, 3).Value = "Contact";
            ws.Cell(1, 4).Value = "Email";
            ws.Cell(1, 5).Value = "Lifecycle";
            ws.Cell(1, 6).Value = "Value Tier";
            ws.Cell(1, 7).Value = "At Risk";
            ws.Cell(1, 8).Value = "Last Contact";

            var headerRow = ws.Range("A1:H1");
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor =
                XLColor.FromHtml("#1e3a8a");
            headerRow.Style.Font.FontColor = XLColor.White;

            int row = 2;
            foreach (var c in customers)
            {
                ws.Cell(row, 1).Value = c.Id;
                ws.Cell(row, 2).Value = c.CompanyName;
                ws.Cell(row, 3).Value = c.ContactName;
                ws.Cell(row, 4).Value = c.Email;
                ws.Cell(row, 5).Value = c.LifecycleStage;
                ws.Cell(row, 6).Value = c.ValueTier;
                ws.Cell(row, 7).Value = c.IsAtRisk ? "Yes" : "No";
                ws.Cell(row, 8).Value = c.LastContactDate
                    .ToString("yyyy-MM-dd");
                row++;
            }

            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }

        public async Task<byte[]> ExportDealsCsvAsync()
        {
            var deals = await _db.Deals
                .Include(d => d.Customer)
                .Include(d => d.Owner)
                .ToListAsync();

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Deals");

            ws.Cell(1, 1).Value = "ID";
            ws.Cell(1, 2).Value = "Title";
            ws.Cell(1, 3).Value = "Customer";
            ws.Cell(1, 4).Value = "Stage";
            ws.Cell(1, 5).Value = "Value (PKR)";
            ws.Cell(1, 6).Value = "Owner";
            ws.Cell(1, 7).Value = "Close Date";

            var headerRow = ws.Range("A1:G1");
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor =
                XLColor.FromHtml("#1e3a8a");
            headerRow.Style.Font.FontColor = XLColor.White;

            int row = 2;
            foreach (var d in deals)
            {
                ws.Cell(row, 1).Value = d.Id;
                ws.Cell(row, 2).Value = d.Title;
                ws.Cell(row, 3).Value = d.Customer?.CompanyName ?? "-";
                ws.Cell(row, 4).Value = d.Stage;
                ws.Cell(row, 5).Value = (double)d.EstimatedValue;
                ws.Cell(row, 6).Value = d.Owner?.FullName ?? "-";
                ws.Cell(row, 7).Value = d.ExpectedCloseDate
                    .ToString("yyyy-MM-dd");
                row++;
            }

            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }

        public async Task<byte[]> ExportTasksCsvAsync()
        {
            var tasks = await _db.Tasks
                .Include(t => t.AssignedTo)
                .ToListAsync();

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Tasks");

            ws.Cell(1, 1).Value = "ID";
            ws.Cell(1, 2).Value = "Title";
            ws.Cell(1, 3).Value = "Priority";
            ws.Cell(1, 4).Value = "Status";
            ws.Cell(1, 5).Value = "Assigned To";
            ws.Cell(1, 6).Value = "Due Date";
            ws.Cell(1, 7).Value = "Est. Hours";

            var headerRow = ws.Range("A1:G1");
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor =
                XLColor.FromHtml("#1e3a8a");
            headerRow.Style.Font.FontColor = XLColor.White;

            int row = 2;
            foreach (var t in tasks)
            {
                ws.Cell(row, 1).Value = t.Id;
                ws.Cell(row, 2).Value = t.Title;
                ws.Cell(row, 3).Value = t.Priority;
                ws.Cell(row, 4).Value = t.Status;
                ws.Cell(row, 5).Value = t.AssignedTo?.FullName ?? "-";
                ws.Cell(row, 6).Value = t.DueDate.ToString("yyyy-MM-dd");
                ws.Cell(row, 7).Value = (double)t.EstimatedHours;
                row++;
            }

            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }

        // ── PDF Export ──────────────────────────────────────────
        public async Task<byte[]> ExportSummaryPdfAsync()
        {
            var totalEmployees = await _db.Employees.CountAsync();
            var totalCustomers = await _db.Customers.CountAsync();
            var totalDeals = await _db.Deals.CountAsync();
            var wonDeals = await _db.Deals
                .CountAsync(d => d.Stage == "Won");
            var totalTasks = await _db.Tasks.CountAsync();
            var completedTasks = await _db.Tasks
                .CountAsync(t => t.Status == "Done");
            var revenue = await _db.Deals
                .Where(d => d.Stage == "Won")
                .ToListAsync();
            var totalRevenue = revenue.Sum(d => d.EstimatedValue);

            var doc = new PdfDocument();
            doc.Info.Title = "SmartBiz 360 Summary Report";

            var page = doc.AddPage();
            var gfx = XGraphics.FromPdfPage(page);

            var fontTitle = new XFont("Arial", 20, XFontStyleEx.Bold);
            var fontHeader = new XFont("Arial", 13, XFontStyleEx.Bold);
            var fontBody = new XFont("Arial", 11, XFontStyleEx.Regular);
            var fontSmall = new XFont("Arial", 9, XFontStyleEx.Regular);

            // Header background
            gfx.DrawRectangle(XBrushes.DarkBlue, 0, 0, page.Width, 80);

            // Title
            gfx.DrawString("SmartBiz 360", fontTitle,
                XBrushes.White,
                new XRect(0, 15, page.Width, 40),
                XStringFormats.TopCenter);

            gfx.DrawString("Business Intelligence Summary Report",
                fontBody, XBrushes.LightGray,
                new XRect(0, 45, page.Width, 30),
                XStringFormats.TopCenter);

            // Date
            gfx.DrawString(
                $"Generated: {DateTime.Now:MMMM dd, yyyy HH:mm}",
                fontSmall, XBrushes.Gray,
                new XRect(40, 95, page.Width, 20),
                XStringFormats.TopLeft);

            // Section: Key Metrics
            gfx.DrawString("Key Metrics", fontHeader,
                XBrushes.DarkBlue,
                new XRect(40, 125, page.Width, 30),
                XStringFormats.TopLeft);

            gfx.DrawLine(XPens.DarkBlue, 40, 148,
                page.Width - 40, 148);

            // Metrics
            var metrics = new[]
            {
                ("Total Employees", totalEmployees.ToString()),
                ("Total Customers", totalCustomers.ToString()),
                ("Total Deals", totalDeals.ToString()),
                ("Won Deals", wonDeals.ToString()),
                ("Total Tasks", totalTasks.ToString()),
                ("Completed Tasks", completedTasks.ToString()),
                ("Total Revenue (PKR)",
                    $"PKR {totalRevenue:N0}")
            };

            int y = 160;
            bool shade = false;
            foreach (var (label, value) in metrics)
            {
                if (shade)
                    gfx.DrawRectangle(
                        new XSolidBrush(XColor.FromArgb(245, 247, 250)),
                        40, y - 4, page.Width - 80, 22);

                gfx.DrawString(label, fontBody,
                    XBrushes.Black,
                    new XRect(50, y, 300, 20),
                    XStringFormats.TopLeft);

                gfx.DrawString(value, fontBody,
                    XBrushes.DarkBlue,
                    new XRect(300, y, 200, 20),
                    XStringFormats.TopLeft);

                y += 26;
                shade = !shade;
            }

            // Footer
            gfx.DrawRectangle(XBrushes.DarkBlue,
                0, page.Height - 40, page.Width, 40);
            gfx.DrawString(
                "SmartBiz 360 — Confidential Business Report",
                fontSmall, XBrushes.White,
                new XRect(0, page.Height - 28, page.Width, 20),
                XStringFormats.TopCenter);

            using var ms = new MemoryStream();
            doc.Save(ms, false);
            return ms.ToArray();
        }
    }
}