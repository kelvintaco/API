using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Spire.Xls; // Namespace for FreeSpire.XLS
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Extgstate;
using iText.Kernel.Exceptions; // For PdfException

namespace WebSystemMonitoring.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelController : ControllerBase
    {
        public ExcelController()
        {
            // Set EPPlus license context
            ExcelPackage.License.SetNonCommercialOrganization("Local Government Unit");
        }
        // Helper method to convert Excel to PDF with A4 settings
        private void ConvertExcelToPdf(string excelFilePath, string pdfFilePath, string worksheetName)
        {
            try
            {
                using (var workbook = new Workbook())
                {
                    workbook.LoadFromFile(excelFilePath);
                    Console.WriteLine($"FreeSpire.XLS loaded Excel: {excelFilePath}");

                    // Ensure only the specified worksheet is converted
                    var worksheet = workbook.Worksheets.FirstOrDefault(ws => ws.Name.Equals(worksheetName, StringComparison.OrdinalIgnoreCase));
                    if (worksheet == null)
                    {
                        throw new Exception($"{worksheetName} worksheet not found in FreeSpire.XLS workbook.");
                    }

                    // Hide other worksheets
                    foreach (var ws in workbook.Worksheets)
                    {
                        if (!ws.Name.Equals(worksheetName, StringComparison.OrdinalIgnoreCase))
                        {
                            ws.Visibility = WorksheetVisibility.Hidden;
                        }
                    }

                    // Configure A4 page settings
                    var pageSetup = worksheet.PageSetup;
                    pageSetup.PaperSize = PaperSizeType.PaperA4; // Set A4 page size
                    pageSetup.FitToPagesWide = 1; // Fit to 1 page wide
                    pageSetup.FitToPagesTall = 1; // Fit to 1 page tall
                    pageSetup.IsFitToPage = true; // Enable fit-to-page scaling
                    pageSetup.LeftMargin = 0.5f; // 0.5 inch margins
                    pageSetup.RightMargin = 0.5f;
                    pageSetup.TopMargin = 0.5f;
                    pageSetup.BottomMargin = 0.5f;
                    pageSetup.Zoom = 100; // Default zoom, overridden by FitToPages

                    // ICS-specific settings to prevent right-side truncation
                    if (worksheetName.Equals("ICS", StringComparison.OrdinalIgnoreCase))
                    {
                        pageSetup.RightMargin = 0.65f; // Increased right margin for ICS
                        pageSetup.Zoom = 85; // Reduce scaling to fit content
                    }
                    if (worksheetName.Equals("surrender", StringComparison.OrdinalIgnoreCase))
                    {
                        // Additional ICS-specific settings
                        pageSetup.RightMargin = 0.50f; // Ensure fit-to-page is enabled
                        pageSetup.Zoom = 85; // Adjust zoom for ICS
                    }
                    if (worksheetName.Equals("transfer", StringComparison.OrdinalIgnoreCase))
                    {
                        // Additional ICS-specific settings
                        pageSetup.RightMargin = 0.50f; // Ensure fit-to-page is enabled
                        pageSetup.Zoom = 85; // Adjust zoom for ICS
                    }
                    else
                    {
                        pageSetup.RightMargin = 0.5f; // Standard right margin for others
                        pageSetup.Zoom = 100; // Default zoom for others
                    }

                    workbook.SaveToFile(pdfFilePath, FileFormat.PDF);
                    Console.WriteLine($"FreeSpire.XLS saved PDF: {pdfFilePath}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"FreeSpire.XLS failed to convert Excel to PDF: {ex.Message}", ex);
            }
        }

        [HttpPost("generate-ics")]
        public async Task<IActionResult> GenerateIcsExcel([FromBody] ICSData data)
        {
            if (data == null)
            {
                return BadRequest("Request body is null or invalid.");
            }

            Console.WriteLine($"Received ICS data: ICSID={data.ICSID}, ItemCode={data.ItemCode}, IcsDate={data.IcsDate}");

            var templateFileName = "various-form.xlsx";
            var existingFilePath = Path.Combine(Directory.GetCurrentDirectory(), "templates", templateFileName);

            Console.WriteLine($"Looking for template at: {existingFilePath}");
            if (!System.IO.File.Exists(existingFilePath))
            {
                return NotFound($"Template file not found at: {existingFilePath}");
            }

            var tempExcelFileName = $"ICSData_{Guid.NewGuid().ToString("N")}.xlsx";
            var tempExcelFilePath = Path.Combine(Path.GetTempPath(), tempExcelFileName);
            var tempPdfFileName = $"ICSData_{Guid.NewGuid().ToString("N")}.pdf";
            var tempPdfFilePath = Path.Combine(Path.GetTempPath(), tempPdfFileName);
            var securedPdfFilePath = $"{tempPdfFilePath}_secured.pdf";

            try
            {
                // Step 1: Generate Excel file using EPPlus
                using (var package = new ExcelPackage(new FileInfo(existingFilePath)))
                {
                    // Log available worksheets
                    var worksheetNames = package.Workbook.Worksheets.Select(ws => ws.Name).ToList();
                    Console.WriteLine($"Available worksheets: {string.Join(", ", worksheetNames)}");

                    var worksheet = package.Workbook.Worksheets.FirstOrDefault(ws => ws.Name.Equals("ICS", StringComparison.OrdinalIgnoreCase));
                    if (worksheet == null)
                    {
                        return BadRequest("Worksheet 'ICS' not found!");
                    }

                    // Updated cell assignments based on document structure
                    worksheet.Cells["J9"].Value = data.ICSID; // ICS No.
                    worksheet.Cells["B13"].Value = data.Qty; // Quantity
                    worksheet.Cells["I13"].Value = data.ItemCode; // Inventory Item No.
                    worksheet.Cells["E13"].Value = data.Description; // Description
                    worksheet.Cells["C13"].Value = data.ICSPrice; // Unit Cost
                    worksheet.Cells["C8"].Value = data.ICSName;
                    worksheet.Cells["G39"].Value = data.ICSName;
                    worksheet.Cells["J13"].Value = data.LifeTime; // Estimated Useful Life
                    worksheet.Cells["C47"].Value = data.IcsDate.ToString("yyyy-MM-dd"); // Date (Received from)
                    worksheet.Cells["H47"].Value = data.IcsDate.ToString("yyyy-MM-dd"); // Date (Received by)
                    worksheet.Cells["G43"].Value = data.Position; // Position/Office
                    worksheet.Cells["A2"].Value = "GF"; // Fund Cluster

                    package.SaveAs(new FileInfo(tempExcelFilePath));
                }

                // Validate Excel output
                if (!System.IO.File.Exists(tempExcelFilePath) || new FileInfo(tempExcelFilePath).Length == 0)
                {
                    throw new Exception("EPPlus failed to generate a valid Excel file.");
                }
                try
                {
                    using (var package = new ExcelPackage(new FileInfo(tempExcelFilePath)))
                    {
                        if (package.Workbook.Worksheets.Count == 0)
                        {
                            throw new Exception("Generated Excel file is invalid or empty.");
                        }
                        Console.WriteLine($"Excel file validated: {tempExcelFilePath}, Worksheets: {package.Workbook.Worksheets.Count}");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Generated Excel file is corrupted: {ex.Message}", ex);
                }

                // Step 2: Convert Excel to PDF using FreeSpire.XLS
                
                try
                {
                    using (var workbook = new Workbook())
                    {
                        workbook.LoadFromFile(tempExcelFilePath);
                        Console.WriteLine($"FreeSpire.XLS loaded Excel: {tempExcelFilePath}");

                        // Ensure only the ICS worksheet is converted
                        var worksheet = workbook.Worksheets.FirstOrDefault(ws => ws.Name.Equals("ICS", StringComparison.OrdinalIgnoreCase));
                        if (worksheet == null)
                        {
                            throw new Exception("ICS worksheet not found in FreeSpire.XLS workbook.");
                        }

                        // Hide other worksheets
                        foreach (var ws in workbook.Worksheets)
                        {
                            if (!ws.Name.Equals("ICS", StringComparison.OrdinalIgnoreCase))
                            {
                                ws.Visibility = WorksheetVisibility.Hidden;
                            }
                        }

                        workbook.SaveToFile(tempPdfFilePath, FileFormat.PDF);
                        Console.WriteLine($"FreeSpire.XLS saved PDF: {tempPdfFilePath}");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"FreeSpire.XLS failed to convert Excel to PDF: {ex.Message}", ex);
                }
                ConvertExcelToPdf(tempExcelFilePath, tempPdfFilePath, "ICS");
                // Validate FreeSpire.XLS output
                if (!System.IO.File.Exists(tempPdfFilePath) || new FileInfo(tempPdfFilePath).Length == 0)
                {
                    throw new Exception("FreeSpire.XLS failed to generate a valid PDF file.");
                }
                try
                {
                    using (var pdfReader = new PdfReader(tempPdfFilePath))
                    {
                        using (var pdfDoc = new PdfDocument(pdfReader))
                        {
                            if (pdfDoc.GetNumberOfPages() == 0)
                            {
                                throw new Exception("FreeSpire.XLS generated an empty or invalid PDF.");
                            }
                            Console.WriteLine($"PDF file validated: {tempPdfFilePath}, Pages: {pdfDoc.GetNumberOfPages()}");
                        }
                    }
                }
                catch (PdfException ex)
                {
                    throw new Exception($"FreeSpire.XLS generated a corrupted PDF: {ex.Message}", ex);
                }

                // Step 3: Use iText to add metadata and watermark
                using (var pdfReader = new PdfReader(tempPdfFilePath))
                using (var pdfWriter = new PdfWriter(new FileStream(securedPdfFilePath, FileMode.Create, FileAccess.Write)))
                using (var pdfDoc = new PdfDocument(pdfReader, pdfWriter))
                {
                    // Step 3.1: Add metadata
                    try
                    {
                        var info = pdfDoc.GetDocumentInfo();
                        info.SetTitle("ICS Document");
                        info.SetAuthor("WebSystemMonitoring");
                        info.SetCreator("WebSystemMonitoring App");
                        info.SetSubject("ICS Data Report");
                        info.SetKeywords("ICS, Secure, Confidential");
                        Console.WriteLine("PDF metadata set successfully.");
                    }
                    catch (PdfException ex)
                    {
                        throw new Exception($"Failed to set PDF metadata: {ex.Message}", ex);
                    }

                    // Step 3.2: Add watermark
                    try
                    {
                        for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                        {
                            var page = pdfDoc.GetPage(i);
                            if (page == null) continue;

                            var canvas = new PdfCanvas(page);
                            canvas.SaveState();

                            canvas.SetFillColor(new DeviceRgb(200, 200, 200));
                            var transparency = new PdfExtGState().SetFillOpacity(0.3f);
                            canvas.SetExtGState(transparency);

                            try
                            {
                                var font = iText.Kernel.Font.PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);
                                canvas.BeginText()
                                    .SetFontAndSize(font, 50)
                                    .MoveText(100, 400)
                                    .ShowText($"CONFIDENTIAL - ICS {data.ICSID}")
                                    .EndText();
                            }
                            catch (PdfException ex)
                            {
                                Console.WriteLine($"Warning: Failed to apply watermark on page {i}: {ex.Message}");
                            }

                            canvas.RestoreState();
                        }
                        Console.WriteLine("PDF watermark applied successfully.");
                    }
                    catch (PdfException ex)
                    {
                        throw new Exception($"Failed to add watermark to PDF: {ex.Message}", ex);
                    }
                }

                // Step 4: Return the secured PDF
                if (!System.IO.File.Exists(securedPdfFilePath) || new FileInfo(securedPdfFilePath).Length == 0)
                {
                    throw new Exception("Failed to generate the final PDF file.");
                }

                var fileBytes = await System.IO.File.ReadAllBytesAsync(securedPdfFilePath);
                Console.WriteLine($"Returning PDF: {securedPdfFilePath}, Size: {fileBytes.Length} bytes");
                Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
                Response.Headers.Add("Pragma", "no-cache");
                Response.Headers.Add("Expires", "0");
                return File(fileBytes, "application/pdf", "ICSData.pdf");
            }
            catch (PdfException ex)
            {
                Console.WriteLine($"PdfException in GenerateIcsExcel: {ex.Message}\nStackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}\nInner StackTrace: {ex.InnerException.StackTrace}");
                }
                return StatusCode(500, $"PDF processing error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GenerateIcsExcel: {ex.Message}\nStackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}\nInner StackTrace: {ex.InnerException.StackTrace}");
                }
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
            finally
            {
                TryDeleteFile(tempExcelFilePath);
                TryDeleteFile(tempPdfFilePath);
                TryDeleteFile(securedPdfFilePath);
            }
        }

        [HttpPost("generate-surrender")]
        public async Task<IActionResult> GenerateSurrenderExcel([FromBody] SurrenderData data)
        {
            if (data == null)
            {
                return BadRequest("Request body is null or invalid.");
            }

            Console.WriteLine($"Received Surrender data: Quantity={data.Quantity}, ItemCode={data.ItemCode}, ParDate={data.ParDate}");

            var templateFileName = "various-form.xlsx";
            var existingFilePath = Path.Combine(Directory.GetCurrentDirectory(), "templates", templateFileName);

            Console.WriteLine($"Looking for template at: {existingFilePath}");
            if (!System.IO.File.Exists(existingFilePath))
            {
                return NotFound($"Template file not found at: {existingFilePath}");
            }

            var tempExcelFileName = $"SurrenderData_{Guid.NewGuid().ToString("N")}.xlsx";
            var tempExcelFilePath = Path.Combine(Path.GetTempPath(), tempExcelFileName);
            var tempPdfFileName = $"SurrenderData_{Guid.NewGuid().ToString("N")}.pdf";
            var tempPdfFilePath = Path.Combine(Path.GetTempPath(), tempPdfFileName);
            var securedPdfFilePath = $"{tempPdfFilePath}_secured.pdf";

            try
            {
                // Step 1: Generate Excel file using EPPlus
                using (var package = new ExcelPackage(new FileInfo(existingFilePath)))
                {
                    // Log available worksheets
                    var worksheetNames = package.Workbook.Worksheets.Select(ws => ws.Name).ToList();
                    Console.WriteLine($"Available worksheets: {string.Join(", ", worksheetNames)}");

                    var worksheet = package.Workbook.Worksheets.FirstOrDefault(ws => ws.Name.Equals("surrender", StringComparison.OrdinalIgnoreCase));
                    if (worksheet == null)
                    {
                        return BadRequest("Worksheet 'surrender' not found!");
                    }

                    // Updated cell assignments based on document structure
                    worksheet.Cells["A9"].Value = data.Quantity; // QTY
                    worksheet.Cells["B9"].Value = data.ItemCode; // UNIT
                    worksheet.Cells["C9"].Value = data.ItemName; // DESCRIPTION
                    worksheet.Cells["D9"].Value = data.ParDate.ToString("yyyy-MM-dd"); // DATE ACQUIRED
                    worksheet.Cells["E9"].Value = data.ParID; // PROPERTY NO
                    worksheet.Cells["F9"].Value = data.Value; // UNIT VALUE
                    worksheet.Cells["G9"].Value = data.Price; // TOTAL VALUE
                    worksheet.Cells["A35"].Value = data.ParName; // SURRENDERED BY
                    worksheet.Cells["A40"].Value = data.Condition; // ITEM CONDITION

                    if (data.IsClassification1) worksheet.Cells["A25"].Value = "/"; // Office Equipment
                    if (data.IsClassification2) worksheet.Cells["A26"].Value = "/"; // Furniture & Fixtures
                    if (data.IsClassification3) worksheet.Cells["A27"].Value = "/"; // IT Equipment
                    if (data.IsClassification4) worksheet.Cells["A28"].Value = "/"; // Other Machinery & Equipment
                    if (data.IsClassification5) worksheet.Cells["A29"].Value = "/"; // Communication Equipment

                    if (data.CopiesEndUser) worksheet.Cells["G25"].Value = "/"; // End User
                    if (data.CopiesGSO) worksheet.Cells["G26"].Value = "/"; // GSO

                    package.SaveAs(new FileInfo(tempExcelFilePath));
                }

                // Validate Excel output
                if (!System.IO.File.Exists(tempExcelFilePath) || new FileInfo(tempExcelFilePath).Length == 0)
                {
                    throw new Exception("EPPlus failed to generate a valid Excel file.");
                }
                try
                {
                    using (var package = new ExcelPackage(new FileInfo(tempExcelFilePath)))
                    {
                        if (package.Workbook.Worksheets.Count == 0)
                        {
                            throw new Exception("Generated Excel file is invalid or empty.");
                        }
                        Console.WriteLine($"Excel file validated: {tempExcelFilePath}, Worksheets: {package.Workbook.Worksheets.Count}");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Generated Excel file is corrupted: {ex.Message}", ex);
                }

                // Step 2: Convert Excel to PDF using FreeSpire.XLS
                try
                {
                    using (var workbook = new Workbook())
                    {
                        workbook.LoadFromFile(tempExcelFilePath);
                        Console.WriteLine($"FreeSpire.XLS loaded Excel: {tempExcelFilePath}");

                        // Ensure only the surrender worksheet is converted
                        var worksheet = workbook.Worksheets.FirstOrDefault(ws => ws.Name.Equals("surrender", StringComparison.OrdinalIgnoreCase));
                        if (worksheet == null)
                        {
                            throw new Exception("Surrender worksheet not found in FreeSpire.XLS workbook.");
                        }

                        // Hide other worksheets
                        foreach (var ws in workbook.Worksheets)
                        {
                            if (!ws.Name.Equals("surrender", StringComparison.OrdinalIgnoreCase))
                            {
                                ws.Visibility = WorksheetVisibility.Hidden;
                            }
                        }

                        workbook.SaveToFile(tempPdfFilePath, FileFormat.PDF);
                        Console.WriteLine($"FreeSpire.XLS saved PDF: {tempPdfFilePath}");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"FreeSpire.XLS failed to convert Excel to PDF: {ex.Message}", ex);
                }
                ConvertExcelToPdf(tempExcelFilePath, tempPdfFilePath, "surrender");
                // Validate FreeSpire.XLS output
                if (!System.IO.File.Exists(tempPdfFilePath) || new FileInfo(tempPdfFilePath).Length == 0)
                {
                    throw new Exception("FreeSpire.XLS failed to generate a valid PDF file.");
                }
                try
                {
                    using (var pdfReader = new PdfReader(tempPdfFilePath))
                    {
                        using (var pdfDoc = new PdfDocument(pdfReader))
                        {
                            if (pdfDoc.GetNumberOfPages() == 0)
                            {
                                throw new Exception("FreeSpire.XLS generated an empty or invalid PDF.");
                            }
                            Console.WriteLine($"PDF file validated: {tempPdfFilePath}, Pages: {pdfDoc.GetNumberOfPages()}");
                        }
                    }
                }
                catch (PdfException ex)
                {
                    throw new Exception($"FreeSpire.XLS generated a corrupted PDF: {ex.Message}", ex);
                }

                // Step 3: Use iText to add metadata and watermark
                using (var pdfReader = new PdfReader(tempPdfFilePath))
                using (var pdfWriter = new PdfWriter(new FileStream(securedPdfFilePath, FileMode.Create, FileAccess.Write)))
                using (var pdfDoc = new PdfDocument(pdfReader, pdfWriter))
                {
                    // Step 3.1: Add metadata
                    try
                    {
                        var info = pdfDoc.GetDocumentInfo();
                        info.SetTitle("Surrender Document");
                        info.SetAuthor("WebSystemMonitoring");
                        info.SetCreator("WebSystemMonitoring App");
                        info.SetSubject("Surrender Data Report");
                        info.SetKeywords("Surrender, Secure, Confidential");
                        Console.WriteLine("PDF metadata set successfully.");
                    }
                    catch (PdfException ex)
                    {
                        throw new Exception($"Failed to set PDF metadata: {ex.Message}", ex);
                    }

                    // Step 3.2: Add watermark
                    try
                    {
                        for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                        {
                            var page = pdfDoc.GetPage(i);
                            if (page == null) continue;

                            var canvas = new PdfCanvas(page);
                            canvas.SaveState();

                            canvas.SetFillColor(new DeviceRgb(200, 200, 200));
                            var transparency = new PdfExtGState().SetFillOpacity(0.3f);
                            canvas.SetExtGState(transparency);

                            try
                            {
                                var font = iText.Kernel.Font.PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);
                                canvas.BeginText()
                                    .SetFontAndSize(font, 50)
                                    .MoveText(100, 400)
                                    .ShowText($"CONFIDENTIAL - Surrender {data.ParID}")
                                    .EndText();
                            }
                            catch (PdfException ex)
                            {
                                Console.WriteLine($"Warning: Failed to apply watermark on page {i}: {ex.Message}");
                            }

                            canvas.RestoreState();
                        }
                        Console.WriteLine("PDF watermark applied successfully.");
                    }
                    catch (PdfException ex)
                    {
                        throw new Exception($"Failed to add watermark to PDF: {ex.Message}", ex);
                    }
                }

                // Step 4: Return the secured PDF
                if (!System.IO.File.Exists(securedPdfFilePath) || new FileInfo(securedPdfFilePath).Length == 0)
                {
                    throw new Exception("Failed to generate the final PDF file.");
                }

                var fileBytes = await System.IO.File.ReadAllBytesAsync(securedPdfFilePath);
                Console.WriteLine($"Returning PDF: {securedPdfFilePath}, Size: {fileBytes.Length} bytes");
                Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
                Response.Headers.Add("Pragma", "no-cache");
                Response.Headers.Add("Expires", "0");
                return File(fileBytes, "application/pdf", "SurrenderData.pdf");
            }
            catch (PdfException ex)
            {
                Console.WriteLine($"PdfException in GenerateSurrenderExcel: {ex.Message}\nStackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}\nInner StackTrace: {ex.InnerException.StackTrace}");
                }
                return StatusCode(500, $"PDF processing error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GenerateSurrenderExcel: {ex.Message}\nStackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}\nInner StackTrace: {ex.InnerException.StackTrace}");
                }
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
            finally
            {
                TryDeleteFile(tempExcelFilePath);
                TryDeleteFile(tempPdfFilePath);
                TryDeleteFile(securedPdfFilePath);
            }
        }

        [HttpPost("generate-par")]
        public async Task<IActionResult> GenerateParExcel([FromBody] ParData data)
        {
            if (data == null)
            {
                return BadRequest("Request body is null or invalid.");
            }

            if (data.ParDate == default)
            {
                return BadRequest("ParDate is required and must be a valid date.");
            }

            Console.WriteLine($"Received PAR data: ParID={data.ParID}, ItemCode={data.ItemCode}, ParDate={data.ParDate}");

            var templateFileName = "various-form.xlsx";
            var existingFilePath = Path.Combine(Directory.GetCurrentDirectory(), "templates", templateFileName);

            Console.WriteLine($"Looking for template at: {existingFilePath}");
            if (!System.IO.File.Exists(existingFilePath))
            {
                return NotFound($"Template file not found at: {existingFilePath}");
            }

            var tempExcelFileName = $"PARData_{Guid.NewGuid().ToString("N")}.xlsx";
            var tempExcelFilePath = Path.Combine(Path.GetTempPath(), tempExcelFileName);
            var tempPdfFileName = $"PARData_{Guid.NewGuid().ToString("N")}.pdf";
            var tempPdfFilePath = Path.Combine(Path.GetTempPath(), tempPdfFileName);
            var securedPdfFilePath = $"{tempPdfFilePath}_secured.pdf";

            try
            {
                // Step 1: Generate Excel file using EPPlus
                using (var package = new ExcelPackage(new FileInfo(existingFilePath)))
                {
                    // Log available worksheets
                    var worksheetNames = package.Workbook.Worksheets.Select(ws => ws.Name).ToList();
                    Console.WriteLine($"Available worksheets: {string.Join(", ", worksheetNames)}");

                    var worksheet = package.Workbook.Worksheets.FirstOrDefault(ws => ws.Name.Equals("PAR", StringComparison.OrdinalIgnoreCase));
                    if (worksheet == null)
                    {
                        return BadRequest("Worksheet 'PAR' not found!");
                    }

                    // Cell assignments based on document structure
                    worksheet.Cells["F6"].Value = data.ParID; // PAR NO.
                    worksheet.Cells["A9"].Value = data.ParQty; // QTY
                    worksheet.Cells["B9"].Value = data.Unit; // UNIT
                    //worksheet.Cells["B9"].Value = data.ItemCode; // UNIT
                    //worksheet.Cells["C9"].Value = data.ItemName;
                    worksheet.Cells["C9"].Value = $"{data.ItemName} - {data.ItemDetails}";// DESCRIPTION
                    worksheet.Cells["D9"].Value = data.ParDate.ToString("yyyy-MM-dd"); // DATE ACQUIRED
                    worksheet.Cells["E9"].Value = data.ItemCode; // PROPERTY NO
                    worksheet.Cells["F9"].Value = data.value; // UNIT VALUE
                    worksheet.Cells["E40"].Value = data.ParName; // RECEIVED BY
                    worksheet.Cells["C36"].Value = data.ParDate.ToString("yyyy-MM-dd"); // Date (Reference)
                    worksheet.Cells["C35"].Value = data.RefNo; // Reference Check #
                    worksheet.Cells["D47"].Value = data.head; // OFFICE HEAD

                    if (data.IsClassification1) worksheet.Cells["A29"].Value = "/"; // Office Equipment
                    if (data.IsClassification2) worksheet.Cells["A30"].Value = "/"; // Furniture & Fixtures
                    if (data.IsClassification3) worksheet.Cells["A31"].Value = "/"; // IT Equipment
                    if (data.IsClassification4) worksheet.Cells["A32"].Value = "/"; // Other Machinery & Equipment
                    if (data.IsClassification5) worksheet.Cells["A33"].Value = "/"; // Communication Equipment

                    if (data.Copies1) worksheet.Cells["G33"].Value = "/"; // Requisitioner c/o Property Custodian
                    if (data.Copies2) worksheet.Cells["G34"].Value = "/"; // GSO
                    if (data.Copies3) worksheet.Cells["G35"].Value = "/"; // Accounting Office
                    if (data.Copies4) worksheet.Cells["G36"].Value = "/"; // Disbursement Voucher

                    worksheet.Cells["D29"].Value = data.FundType.Contains("GF") ? "/" : ""; // GF
                    worksheet.Cells["D30"].Value = data.FundType.Contains("SEF") ? "/" : ""; // SEF
                    worksheet.Cells["G29"].Value = data.FundType.Contains("Trust Fund") ? "/" : ""; // Trust Fund
                    worksheet.Cells["F30"].Value = data.FundType.Contains("Other") ? "/" : ""; // Other
                    if (data.FundType.Contains("Other") && !string.Equals(data.FundType, "Other", StringComparison.OrdinalIgnoreCase))
                    {
                        worksheet.Cells["F30"].Value = data.FundType; // Specify other fund type
                    }

                    package.SaveAs(new FileInfo(tempExcelFilePath));
                }

                // Validate Excel output
                if (!System.IO.File.Exists(tempExcelFilePath) || new FileInfo(tempExcelFilePath).Length == 0)
                {
                    throw new Exception("EPPlus failed to generate a valid Excel file.");
                }
                try
                {
                    using (var package = new ExcelPackage(new FileInfo(tempExcelFilePath)))
                    {
                        if (package.Workbook.Worksheets.Count == 0)
                        {
                            throw new Exception("Generated Excel file is invalid or empty.");
                        }
                        Console.WriteLine($"Excel file validated: {tempExcelFilePath}, Worksheets: {package.Workbook.Worksheets.Count}");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Generated Excel file is corrupted: {ex.Message}", ex);
                }

                // Step 2: Convert Excel to PDF using FreeSpire.XLS
                try
                {
                    using (var workbook = new Workbook())
                    {
                        workbook.LoadFromFile(tempExcelFilePath);
                        Console.WriteLine($"FreeSpire.XLS loaded Excel: {tempExcelFilePath}");

                        // Ensure only the PAR worksheet is converted
                        var worksheet = workbook.Worksheets.FirstOrDefault(ws => ws.Name.Equals("PAR", StringComparison.OrdinalIgnoreCase));
                        if (worksheet == null)
                        {
                            throw new Exception("PAR worksheet not found in FreeSpire.XLS workbook.");
                        }

                        // Hide other worksheets
                        foreach (var ws in workbook.Worksheets)
                        {
                            if (!ws.Name.Equals("PAR", StringComparison.OrdinalIgnoreCase))
                            {
                                ws.Visibility = WorksheetVisibility.Hidden;
                            }
                        }

                        workbook.SaveToFile(tempPdfFilePath, FileFormat.PDF);
                        Console.WriteLine($"FreeSpire.XLS saved PDF: {tempPdfFilePath}");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"FreeSpire.XLS failed to convert Excel to PDF: {ex.Message}", ex);
                }

                // Validate FreeSpire.XLS output
                if (!System.IO.File.Exists(tempPdfFilePath) || new FileInfo(tempPdfFilePath).Length == 0)
                {
                    throw new Exception("FreeSpire.XLS failed to generate a valid PDF file.");
                }
                try
                {
                    using (var pdfReader = new PdfReader(tempPdfFilePath))
                    {
                        using (var pdfDoc = new PdfDocument(pdfReader))
                        {
                            if (pdfDoc.GetNumberOfPages() == 0)
                            {
                                throw new Exception("FreeSpire.XLS generated an empty or invalid PDF.");
                            }
                            Console.WriteLine($"PDF file validated: {tempPdfFilePath}, Pages: {pdfDoc.GetNumberOfPages()}");
                        }
                    }
                }
                catch (PdfException ex)
                {
                    throw new Exception($"FreeSpire.XLS generated a corrupted PDF: {ex.Message}", ex);
                }

                // Step 3: Use iText to add metadata and watermark
                using (var pdfReader = new PdfReader(tempPdfFilePath))
                using (var pdfWriter = new PdfWriter(new FileStream(securedPdfFilePath, FileMode.Create, FileAccess.Write)))
                using (var pdfDoc = new PdfDocument(pdfReader, pdfWriter))
                {
                    // Step 3.1: Add metadata
                    try
                    {
                        var info = pdfDoc.GetDocumentInfo();
                        info.SetTitle("PAR Document");
                        info.SetAuthor("WebSystemMonitoring");
                        info.SetCreator("WebSystemMonitoring App");
                        info.SetSubject("PAR Data Report");
                        info.SetKeywords("PAR, Secure, Confidential");
                        Console.WriteLine("PDF metadata set successfully.");
                    }
                    catch (PdfException ex)
                    {
                        throw new Exception($"Failed to set PDF metadata: {ex.Message}", ex);
                    }

                    // Step 3.2: Add watermark
                    try
                    {
                        for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                        {
                            var page = pdfDoc.GetPage(i);
                            if (page == null) continue;

                            var canvas = new PdfCanvas(page);
                            canvas.SaveState();

                            canvas.SetFillColor(new DeviceRgb(200, 200, 200));
                            var transparency = new PdfExtGState().SetFillOpacity(0.3f);
                            canvas.SetExtGState(transparency);

                            try
                            {
                                var font = iText.Kernel.Font.PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);
                                canvas.BeginText()
                                    .SetFontAndSize(font, 50)
                                    .MoveText(100, 400)
                                    .ShowText($"CONFIDENTIAL - PAR {data.ParID}")
                                    .EndText();
                            }
                            catch (PdfException ex)
                            {
                                Console.WriteLine($"Warning: Failed to apply watermark on page {i}: {ex.Message}");
                            }

                            canvas.RestoreState();
                        }
                        Console.WriteLine("PDF watermark applied successfully.");
                    }
                    catch (PdfException ex)
                    {
                        throw new Exception($"Failed to add watermark to PDF: {ex.Message}", ex);
                    }
                }

                // Step 4: Return the secured PDF
                if (!System.IO.File.Exists(securedPdfFilePath) || new FileInfo(securedPdfFilePath).Length == 0)
                {
                    throw new Exception("Failed to generate the final PDF file.");
                }

                var fileBytes = await System.IO.File.ReadAllBytesAsync(securedPdfFilePath);
                Console.WriteLine($"Returning PDF: {securedPdfFilePath}, Size: {fileBytes.Length} bytes");
                Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
                Response.Headers.Add("Pragma", "no-cache");
                Response.Headers.Add("Expires", "0");
                return File(fileBytes, "application/pdf", "PARData.pdf");
            }
            catch (PdfException ex)
            {
                Console.WriteLine($"PdfException in GenerateParExcel: {ex.Message}\nStackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}\nInner StackTrace: {ex.InnerException.StackTrace}");
                }
                return StatusCode(500, $"PDF processing error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GenerateParExcel: {ex.Message}\nStackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}\nInner StackTrace: {ex.InnerException.StackTrace}");
                }
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
            finally
            {
                TryDeleteFile(tempExcelFilePath);
                TryDeleteFile(tempPdfFilePath);
                TryDeleteFile(securedPdfFilePath);
            }
        }

        [HttpPost("generate-transfer")]
        public async Task<IActionResult> GenerateTransferExcel([FromBody] TransferData data)
        {
            if (data == null)
            {
                return BadRequest("Request body is null or invalid.");
            }

            Console.WriteLine($"Received Transfer data: PtrId={data.PtrId}, TransferType={data.TransferType}");

            var templateFileName = "various-form.xlsx";
            var existingFilePath = Path.Combine(Directory.GetCurrentDirectory(), "templates", templateFileName);

            Console.WriteLine($"Looking for template at: {existingFilePath}");
            if (!System.IO.File.Exists(existingFilePath))
            {
                return NotFound($"Template file not found at: {existingFilePath}");
            }

            var tempExcelFileName = $"TransferData_{Guid.NewGuid().ToString("N")}.xlsx";
            var tempExcelFilePath = Path.Combine(Path.GetTempPath(), tempExcelFileName);
            var tempPdfFileName = $"TransferData_{Guid.NewGuid().ToString("N")}.pdf";
            var tempPdfFilePath = Path.Combine(Path.GetTempPath(), tempPdfFileName);
            var securedPdfFilePath = $"{tempPdfFilePath}_secured.pdf";

            try
            {
                // Step 1: Generate Excel file using EPPlus
                using (var package = new ExcelPackage(new FileInfo(existingFilePath)))
                {
                    // Log available worksheets
                    var worksheetNames = package.Workbook.Worksheets.Select(ws => ws.Name).ToList();
                    Console.WriteLine($"Available worksheets: {string.Join(", ", worksheetNames)}");

                    var worksheet = package.Workbook.Worksheets.FirstOrDefault(ws => ws.Name.Equals("Transfer", StringComparison.OrdinalIgnoreCase));
                    if (worksheet == null)
                    {
                        return BadRequest("Worksheet 'Transfer' not found!");
                    }

                    // Updated cell assignments based on document structure
                    worksheet.Cells["G6"].Value = data.FundCluster; // Fund Cluster
                    worksheet.Cells["F8"].Value = data.FromName; // From Accountable Officer
                    worksheet.Cells["F9"].Value = data.ToName; // To Accountable Officer
                    worksheet.Cells["I8"].Value = data.PtrId; // PTR No.
                    worksheet.Cells["I9"].Value = data.DateTransferred.ToString("yyyy-MM-dd"); // Date Acquired
                    worksheet.Cells["D6"].Value = data.ItemCode; // Property No.
                    worksheet.Cells["D18"].Value = data.Description; // Description
                    worksheet.Cells["F6"].Value = data.CstCode; // Amount
                    worksheet.Cells["I18"].Value = data.Condition; // Condition of PPE
                    worksheet.Cells["H53"].Value = data.ReceiveName; // Received by
                    worksheet.Cells["A8"].Value = data.TransferType; // Transfer Type
                    worksheet.Cells["B13"].Value = data.TransferType.Contains("Donation") ? "/" : "";
                    worksheet.Cells["E13"].Value = data.TransferType.Contains("Relocate") ? "/" : "";
                    worksheet.Cells["B14"].Value = data.TransferType.Contains("Reassignment") ? "/" : "";
                    worksheet.Cells["E14"].Value = data.TransferType.Contains("Other") ? "/" : "";
                    if (data.TransferType.Contains("Other") && !string.Equals(data.TransferType, "Other", StringComparison.OrdinalIgnoreCase))
                    {
                        worksheet.Cells["G14"].Value = data.TransferType;
                    }
                    worksheet.Cells["A44"].Value = data.ReasonForTransfer;
                    worksheet.Cells["B54"].Value = data.Designation; 
                    worksheet.Cells["F54"].Value = data.designationRelease;
                    worksheet.Cells["H54"].Value = data.designationReceived;
                    worksheet.Cells["A18"].Value = data.dateAcquired; 
                    worksheet.Cells["H18"].Value = data.Amount; 
                    worksheet.Cells["B18"].Value = data.ItemCode;
                    worksheet.Cells["B53"].Value = data.ApprovedBy;
                    worksheet.Cells["F53"].Value = data.ReleasedBy;
                    worksheet.Cells["H53"].Value = data.approvedByDate;
                    worksheet.Cells["H53"].Value = data.releaseByDate;
                    worksheet.Cells["H53"].Value = data.receivedByDate;

                    package.SaveAs(new FileInfo(tempExcelFilePath));
                }

                // Validate Excel output
                if (!System.IO.File.Exists(tempExcelFilePath) || new FileInfo(tempExcelFilePath).Length == 0)
                {
                    throw new Exception("EPPlus failed to generate a valid Excel file.");
                }
                try
                {
                    using (var package = new ExcelPackage(new FileInfo(tempExcelFilePath)))
                    {
                        if (package.Workbook.Worksheets.Count == 0)
                        {
                            throw new Exception("Generated Excel file is invalid or empty.");
                        }
                        Console.WriteLine($"Excel file validated: {tempExcelFilePath}, Worksheets: {package.Workbook.Worksheets.Count}");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Generated Excel file is corrupted: {ex.Message}", ex);
                }

                // Step 2: Convert Excel to PDF using FreeSpire.XLS
                try
                {
                    using (var workbook = new Workbook())
                    {
                        workbook.LoadFromFile(tempExcelFilePath);
                        Console.WriteLine($"FreeSpire.XLS loaded Excel: {tempExcelFilePath}");

                        // Ensure only the Transfer worksheet is converted
                        var worksheet = workbook.Worksheets.FirstOrDefault(ws => ws.Name.Equals("Transfer", StringComparison.OrdinalIgnoreCase));
                        if (worksheet == null)
                        {
                            throw new Exception("Transfer worksheet not found in FreeSpire.XLS workbook.");
                        }

                        // Hide other worksheets
                        foreach (var ws in workbook.Worksheets)
                        {
                            if (!ws.Name.Equals("Transfer", StringComparison.OrdinalIgnoreCase))
                            {
                                ws.Visibility = WorksheetVisibility.Hidden;
                            }
                        }

                        workbook.SaveToFile(tempPdfFilePath, FileFormat.PDF);
                        Console.WriteLine($"FreeSpire.XLS saved PDF: {tempPdfFilePath}");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"FreeSpire.XLS failed to convert Excel to PDF: {ex.Message}", ex);
                }
                ConvertExcelToPdf(tempExcelFilePath, tempPdfFilePath, "Transfer");
                // Validate FreeSpire.XLS output
                if (!System.IO.File.Exists(tempPdfFilePath) || new FileInfo(tempPdfFilePath).Length == 0)
                {
                    throw new Exception("FreeSpire.XLS failed to generate a valid PDF file.");
                }
                try
                {
                    using (var pdfReader = new PdfReader(tempPdfFilePath))
                    {
                        using (var pdfDoc = new PdfDocument(pdfReader))
                        {
                            if (pdfDoc.GetNumberOfPages() == 0)
                            {
                                throw new Exception("FreeSpire.XLS generated an empty or invalid PDF.");
                            }
                            Console.WriteLine($"PDF file validated: {tempPdfFilePath}, Pages: {pdfDoc.GetNumberOfPages()}");
                        }
                    }
                }
                catch (PdfException ex)
                {
                    throw new Exception($"FreeSpire.XLS generated a corrupted PDF: {ex.Message}", ex);
                }

                // Step 3: Use iText to add metadata and watermark
                using (var pdfReader = new PdfReader(tempPdfFilePath))
                using (var pdfWriter = new PdfWriter(new FileStream(securedPdfFilePath, FileMode.Create, FileAccess.Write)))
                using (var pdfDoc = new PdfDocument(pdfReader, pdfWriter))
                {
                    // Step 3.1: Add metadata
                    try
                    {
                        var info = pdfDoc.GetDocumentInfo();
                        info.SetTitle("Transfer Document");
                        info.SetAuthor("WebSystemMonitoring");
                        info.SetCreator("WebSystemMonitoring App");
                        info.SetSubject("Transfer Data Report");
                        info.SetKeywords("Transfer, Secure, Confidential");
                        Console.WriteLine("PDF metadata set successfully.");
                    }
                    catch (PdfException ex)
                    {
                        throw new Exception($"Failed to set PDF metadata: {ex.Message}", ex);
                    }

                    // Step 3.2: Add watermark
                    try
                    {
                        for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                        {
                            var page = pdfDoc.GetPage(i);
                            if (page == null) continue;

                            var canvas = new PdfCanvas(page);
                            canvas.SaveState();

                            canvas.SetFillColor(new DeviceRgb(200, 200, 200));
                            var transparency = new PdfExtGState().SetFillOpacity(0.3f);
                            canvas.SetExtGState(transparency);

                            try
                            {
                                var font = iText.Kernel.Font.PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);
                                canvas.BeginText()
                                    .SetFontAndSize(font, 50)
                                    .MoveText(100, 400)
                                    .ShowText($"CONFIDENTIAL - Transfer {data.PtrId}")
                                    .EndText();
                            }
                            catch (PdfException ex)
                            {
                                Console.WriteLine($"Warning: Failed to apply watermark on page {i}: {ex.Message}");
                            }

                            canvas.RestoreState();
                        }
                        Console.WriteLine("PDF watermark applied successfully.");
                    }
                    catch (PdfException ex)
                    {
                        throw new Exception($"Failed to add watermark to PDF: {ex.Message}", ex);
                    }
                }

                // Step 4: Return the secured PDF
                if (!System.IO.File.Exists(securedPdfFilePath) || new FileInfo(securedPdfFilePath).Length == 0)
                {
                    throw new Exception("Failed to generate the final PDF file.");
                }

                var fileBytes = await System.IO.File.ReadAllBytesAsync(securedPdfFilePath);
                Console.WriteLine($"Returning PDF: {securedPdfFilePath}, Size: {fileBytes.Length} bytes");
                Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
                Response.Headers.Add("Pragma", "no-cache");
                Response.Headers.Add("Expires", "0");
                return File(fileBytes, "application/pdf", "TransferData.pdf");
            }
            catch (PdfException ex)
            {
                Console.WriteLine($"PdfException in GenerateTransferExcel: {ex.Message}\nStackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}\nInner StackTrace: {ex.InnerException.StackTrace}");
                }
                return StatusCode(500, $"PDF processing error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GenerateTransferExcel: {ex.Message}\nStackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}\nInner StackTrace: {ex.InnerException.StackTrace}");
                }
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
            finally
            {
                TryDeleteFile(tempExcelFilePath);
                TryDeleteFile(tempPdfFilePath);
                TryDeleteFile(securedPdfFilePath);
            }
        }

        // Helper method to delete files with retry mechanism
        private void TryDeleteFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
                return;

            const int maxRetries = 5;
            const int delayMs = 1000;

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    System.IO.File.Delete(filePath);
                    Console.WriteLine($"Deleted file: {filePath}");
                    return;
                }
                catch (IOException)
                {
                    if (i == maxRetries - 1)
                        Console.WriteLine($"Failed to delete file {filePath} after {maxRetries} attempts.");
                    Thread.Sleep(delayMs);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting file {filePath}: {ex.Message}");
                    return;
                }
            }
        }
    }

    // ICS Data Model
    public class ICSData
    {
        public int ICSID { get; set; }
        public int ItemCode { get; set; }
        public string Description { get; set; }
        public string CSTCode { get; set; }
        public string ICSName { get; set; }
        public double ICSPrice { get; set; }
        public int LifeTime { get; set; }
        public int Qty { get; set; }
        public DateOnly IcsDate { get; set; }
        public string Position { get; set; }
        public DateOnly ICSSDate { get; set; }
        public DateOnly IcsDateReceived { get; set; }
        public string FundCluster { get; set; }
    }

    // Surrender Data Model
    public class SurrenderData
    {
        public string Quantity { get; set; }
        public int ItemCode { get; set; }
        public string ItemName { get; set; }
        public DateTime ParDate { get; set; }
        public string ParID { get; set; }
        public double Value { get; set; }
        public double Price { get; set; }
        public string ParName { get; set; }
        public string Condition { get; set; }
        public bool IsClassification1 { get; set; }
        public bool IsClassification2 { get; set; }
        public bool IsClassification3 { get; set; }
        public bool IsClassification4 { get; set; }
        public bool IsClassification5 { get; set; }
        public bool CopiesEndUser { get; set; }
        public bool CopiesGSO { get; set; }
        public string? SurName { get; set; }
        public string? DprName { get; set; }
        public string? PrpName { get; set; }
        public string? archiveDate { get; set; }
        public string? ItemCond { get; set; }
        public int SurQTY { get; set; }
        public string Clasification { get; set; }
        public string Copies { get; set; }
    }

    // PAR Data Model
    public class ParData
    {
        public string ParID { get; set; }
        public int ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ParName { get; set; }
        public DateTime ParDate { get; set; }
        public int RefNo { get; set; }
        public int ParQty { get; set; }
        public string Unit { get; set; }
        public bool IsClassification1 { get; set; }
        public bool IsClassification2 { get; set; }
        public bool IsClassification3 { get; set; }
        public bool IsClassification4 { get; set; }
        public bool IsClassification5 { get; set; }
        public bool Copies1 { get; set; }
        public bool Copies2 { get; set; }
        public bool Copies3 { get; set; }
        public bool Copies4 { get; set; }
        public string ItemDetails { get; set; }
        public string FundType { get; set; }
        public float value { get; set; }
        public string? head { get; set; }
        public string? itemunit { get; set; }
    }

    // Transfer Data Model
    public class TransferData
    {
        public int PtrId { get; set; }
        public int ItemCode { get; set; }
        public string Description { get; set; }
        public string CstCode { get; set; }
        public string Name { get; set; }
        public DateOnly DateTransferred { get; set; }
        public string Condition { get; set; }
        public string ReceiveName { get; set; }
        public string TransferType { get; set; }
        public string FundCluster { get; set; }
        public string FromName { get; set; }
        public string ToName { get; set; }
        public string ReasonForTransfer { get; set; }
        public string ApprovedBy { get; set; }
        public string ReleasedBy { get; set; }
        public string Designation { get; set; }
        public string fundccl { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string reason { get; set; }
        public string apprvdBy { get; set; }
        public string designationOf { get; set; }
        public string approvedByDate { get; set; }
        public string releaseBy { get; set; }
        public string designationRelease { get; set; }
        public string releaseByDate { get; set; }
        public string receivedBy { get; set; }
        public string designationReceived { get; set; }
        public string receivedByDate { get; set; }
        public string dateAcquired { get; set; }
        public int Amount { get; set; }
        public string rvName { get; set; }
    }
}