using Microsoft.AspNetCore.Mvc;
using APIFinal.Context;
using APIFinal.Models;
using Microsoft.EntityFrameworkCore;

namespace APIFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransferController : Controller
    {
        private readonly DataContext _dataContext;
        public TransferController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public List<Transfer> GetTransfer()
        {
            return _dataContext.Transfer.ToList();
        }
        [HttpGet("GetLatest")]
        public List<int> GetLatest()
        {
            return _dataContext.Transfer.Select(i => i.PtrId).ToList();
        }
        [HttpGet("Exists/{prtid}")]
        public async Task<IActionResult> CheckPrtIdExists(int prtid)
        {
            bool exists = await _dataContext.Transfer.AnyAsync(c => c.PtrId == prtid);
            return Ok(exists);
        }


        [HttpPost]
        public void Post([FromBody] Transfer transfer)
        {
            _dataContext.Transfer.Add(transfer);
            _dataContext.SaveChanges();
        }
    }
}
