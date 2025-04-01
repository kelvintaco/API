using Microsoft.AspNetCore.Mvc;
using APIFinal.Context;
using APIFinal.Models;
using Microsoft.EntityFrameworkCore;

namespace APIFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustodianController : Controller
    { 
        private readonly DataContext _dataContext;
        public CustodianController (DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public List<Custodian> GetCustodian()
        {
            return _dataContext.Custodians.ToList();
        }

        [HttpGet("GetLatest")]
        public List<string> GetLatest()
        {
            return _dataContext.Custodians.Select(i => i.CSTCode).ToList();
        }

        [HttpGet("Exists/{cstcode}")]
        public async Task<IActionResult> CheckCstCodeExists(string cstcode)
        {
            bool exists = await _dataContext.Custodians.AnyAsync(c => c.CSTCode == cstcode);
            return Ok(exists);
        }
        //[HttpGet("getCstCode/{cstname}")] 
        //public ActionResult<string> GetItemCode(string cstcode)
        //{
        //    var item = _dataContext.Custodians.FirstOrDefault(i => i.CSTCode == cstcode);
        //    if (item == null)
        //    {
        //        return NotFound($"Item with name '{cstcode}' not found.");
        //    }
        //    return Ok(item.CSTName);
        //}
        [HttpGet("getCstCode/{cstname}")]
        public ActionResult<string> GetCstCode(string cstname)
        {
            var code = _dataContext.Custodians.FirstOrDefault(i => i.CSTName == cstname);
            if (code == null)
            {
                return NotFound($"Custodian with name '{cstname}' not found.");
            }
            return Ok(code.CSTCode);
        }

        [HttpPost]
        public void PostCustodian(Custodian custodian)
        {
            _dataContext.Custodians.Add(custodian);
            _dataContext.SaveChanges();
        }
    }
}
