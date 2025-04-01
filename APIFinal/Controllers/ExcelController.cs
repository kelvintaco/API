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
            if (data == null)
            {
                return BadRequest("Request body is null or invalid.");
            }

            Console.WriteLine($"Received data: Quantity={data.Quantity}, ItemCode={data.ItemCode}, ParDate={data.ParDate}");

            var templateFileName = "various-form.xlsx";
            var existingFilePath = Path.Combine(Directory.GetCurrentDirectory(), "templates", templateFileName);

            Console.WriteLine($"Looking for template at: {existingFilePath}");
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

        [HttpPost("generate-par")]
        public async Task<IActionResult> GenerateParExcel([FromBody] ParData data)
        {
            if (data == null)
            {
                return BadRequest("Request body is null or invalid.");
            }

            Console.WriteLine($"Received PAR data: ParID={data.ParID}, ItemCode={data.ItemCode}, ParDate={data.ParDate}");

            var templateFileName = "various-form.xlsx";
            var existingFilePath = Path.Combine(Directory.GetCurrentDirectory(), "templates", templateFileName);

            Console.WriteLine($"Looking for template at: {existingFilePath}");
            if (!System.IO.File.Exists(existingFilePath))
            {
                return NotFound($"Template file not found at: {existingFilePath}");
            }

            var tempFileName = $"PARData_{Guid.NewGuid().ToString("N")}.xlsx";
            var tempFilePath = Path.Combine(Path.GetTempPath(), tempFileName);

            using (var package = new ExcelPackage(new FileInfo(existingFilePath)))
            {
                var worksheet = package.Workbook.Worksheets["PAR"];
                if (worksheet == null)
                {
                    return BadRequest("Worksheet 'PAR' not found!");
                }

                worksheet.Cells["F6"].Value = data.ParID;
                worksheet.Cells["E9"].Value = data.ItemCode;
                worksheet.Cells["C9"].Value = data.ItemName;
                worksheet.Cells["E40"].Value = data.ParName;
                worksheet.Cells["C36"].Value = data.ParDate;
                worksheet.Cells["C35"].Value = data.RefNo;
                worksheet.Cells["A9"].Value = data.ParQty;

                if (data.IsClassification1) worksheet.Cells["A29"].Value = "/";
                if (data.IsClassification2) worksheet.Cells["A30"].Value = "/";
                if (data.IsClassification3) worksheet.Cells["A31"].Value = "/";
                if (data.IsClassification4) worksheet.Cells["A32"].Value = "/";
                if (data.IsClassification5) worksheet.Cells["A33"].Value = "/";

                if (data.IsSourceOfFund1) worksheet.Cells["C2"].Value = "/";
                if (data.IsSourceOfFund2) worksheet.Cells["C3"].Value = "/";
                if (data.IsSourceOfFund3) worksheet.Cells["C4"].Value = "/";
                if (data.IsSourceOfFund4) worksheet.Cells["C5"].Value = data.OtherSourceOfFund;

                package.SaveAs(new FileInfo(tempFilePath));
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(tempFilePath);
            System.IO.File.Delete(tempFilePath);

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PARData.xlsx");
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

            var tempFileName = $"TransferData_{Guid.NewGuid().ToString("N")}.xlsx";
            var tempFilePath = Path.Combine(Path.GetTempPath(), tempFileName);

            using (var package = new ExcelPackage(new FileInfo(existingFilePath)))
            {
                var worksheet = package.Workbook.Worksheets["Transfer"];
                if (worksheet == null)
                {
                    return BadRequest("Worksheet 'Transfer' not found!");
                }
                //fix
                worksheet.Cells["A2"].Value = data.FundCluster;
                worksheet.Cells["B4"].Value = data.FromName;
                worksheet.Cells["C4"].Value = data.ToName;
                worksheet.Cells["D6"].Value = data.ItemCode;
                worksheet.Cells["E6"].Value = data.Description;
                worksheet.Cells["F6"].Value = data.CstCode;
                worksheet.Cells["G6"].Value = data.Name;
                worksheet.Cells["H6"].Value = data.DateTransferred.ToString("yyyy-MM-dd");
                worksheet.Cells["I6"].Value = data.Condition;
                worksheet.Cells["J6"].Value = data.ReceiveName;
                worksheet.Cells["A8"].Value = data.TransferType;
                worksheet.Cells["A10"].Value = data.ReasonForTransfer;
                worksheet.Cells["B12"].Value = data.ApprovedBy;
                worksheet.Cells["C12"].Value = data.ReleasedBy; 
                worksheet.Cells["D12"].Value = data.Designation; 
                worksheet.Cells["E12"].Value = data.PtrId;

                worksheet.Cells["B8"].Value = data.TransferType.Contains("Donation") ? "Yes" : "No";
                worksheet.Cells["C8"].Value = data.TransferType.Contains("Relocate") ? "Yes" : "No"; 
                worksheet.Cells["D8"].Value = data.TransferType.Contains("Reassignment") ? "Yes" : "No"; 
                worksheet.Cells["E8"].Value = data.TransferType.Contains("Other") ? "Yes" : "No";
                if (data.TransferType.Contains("Other"))
                {
                    worksheet.Cells["F8"].Value = data.TransferType;
                }

                package.SaveAs(new FileInfo(tempFilePath));
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(tempFilePath);
            System.IO.File.Delete(tempFilePath);

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TransferData.xlsx");
        }
    }

    public class SurrenderData
    {
        public string Quantity { get; set; }
        public string ItemCode { get; set; }
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
    }

    public class ParData
    {
        public string ParID { get; set; }
        public int ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ParName { get; set; }
        public DateTime ParDate { get; set; }
        public int RefNo { get; set; }
        public int ParQty { get; set; }
        public bool IsClassification1 { get; set; }
        public bool IsClassification2 { get; set; }
        public bool IsClassification3 { get; set; }
        public bool IsClassification4 { get; set; }
        public bool IsClassification5 { get; set; }
        public bool IsSourceOfFund1 { get; set; }
        public bool IsSourceOfFund2 { get; set; }
        public bool IsSourceOfFund3 { get; set; }
        public bool IsSourceOfFund4 { get; set; }
        public string OtherSourceOfFund { get; set; }
    }

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
    }
}