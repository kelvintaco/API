using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.IO;
using System.Threading.Tasks;

namespace WebSystemMonitoring.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelController : ControllerBase
    {
        [HttpPost("generate-surrender")]
        public async Task<IActionResult> GenerateSurrenderExcel([FromBody] SurrenderData data)
        {
            var templateFileName = "various-form.xlsx";
            var existingFilePath = Path.Combine(Directory.GetCurrentDirectory(), "templates", templateFileName);

            if (!System.IO.File.Exists(existingFilePath))
            {
                return NotFound($"Template file not found at: {existingFilePath}");
            }

            var tempFileName = $"surrenderData_{Guid.NewGuid().ToString("N")}.xlsx";
            var tempFilePath = Path.Combine(Path.GetTempPath(), tempFileName);

            using (var package = new ExcelPackage(new FileInfo(existingFilePath)))
            {
                var worksheet = package.Workbook.Worksheets["surrender"];
                if (worksheet == null)
                {
                    return BadRequest("Worksheet 'surrender' not found!");
                }

                worksheet.Cells["A9"].Value = data.Quantity;
                worksheet.Cells["B9"].Value = data.ItemCode;
                worksheet.Cells["C9"].Value = data.ItemName;
                worksheet.Cells["D9"].Value = data.ParDate;
                worksheet.Cells["E9"].Value = data.ParID;
                worksheet.Cells["F9"].Value = data.Value;
                worksheet.Cells["G9"].Value = data.Price;
                worksheet.Cells["A35"].Value = data.ParName;
                worksheet.Cells["H9"].Value = data.Condition;

                if (data.IsClassification1) worksheet.Cells["A25"].Value = "/";
                if (data.IsClassification2) worksheet.Cells["A26"].Value = "/";
                if (data.IsClassification3) worksheet.Cells["A27"].Value = "/";
                if (data.IsClassification4) worksheet.Cells["A28"].Value = "/";
                if (data.IsClassification5) worksheet.Cells["A29"].Value = "/";

                if (data.CopiesEndUser) worksheet.Cells["G25"].Value = "/";
                if (data.CopiesGSO) worksheet.Cells["G26"].Value = "/";

                package.SaveAs(new FileInfo(tempFilePath));
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(tempFilePath);
            System.IO.File.Delete(tempFilePath);

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "surrenderData.xlsx");
        }
    }

    public class SurrenderData
    {
        public string Quantity { get; set; }
        public int ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ParDate { get; set; }
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
    }
}