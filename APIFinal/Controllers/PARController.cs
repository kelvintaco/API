using Microsoft.AspNetCore.Mvc;
using APIFinal.Context;
using APIFinal.Models;
using Microsoft.EntityFrameworkCore;

namespace APIFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PARController : Controller
    {
        private readonly DataContext _dataContext;
        public PARController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public List<PAR> GetPAR()
        {
            return _dataContext.PAR.ToList();
        }
        [HttpGet("byParID")]
        public List<string> GetParID()
        {
            return _dataContext.PAR.Select(i => i.ParID).ToList();
        }
        [HttpGet("byParID/{parid}")]
        public ActionResult<PAR> GetByParID(string parid)
        {
            var par = _dataContext.PAR.FirstOrDefault(x => x.ParID == parid);
            if (par == null)
            {
                return NotFound();
            }
            return Ok(par);
        }


        [HttpGet("Exists/{parId}")]
        public async Task<IActionResult> CheckParIDExists(string parId)
        {
            bool exists = await _dataContext.PAR.AnyAsync(p => p.ParID == parId);
            return Ok(exists);
        }


        [HttpGet("GetLatest")]
        public List<string> GetLatest()
        {
            return _dataContext.PAR.Select(i => i.ParID).ToList();
        }

        [HttpGet("getparname/{parid}")]
        public ActionResult<string> GetParName(string parid)
        {
            var par = _dataContext.PAR.FirstOrDefault(x => x.ParID == parid);
            if (par == null)
            {
                return NotFound();
            }
            return Ok(par.ParName);
        }

        [HttpPost]
        public void Post([FromBody] PAR par)
        {
            _dataContext.PAR.Add(par);
            _dataContext.SaveChanges();
        }

        [HttpDelete("byParID/{parid}")]
        public void DeletebyPARCode(string parid)
        {
            var toarchive = _dataContext.PAR.FirstOrDefault(x => x.ParID == parid);
            _dataContext.PAR.Remove(toarchive);
            _dataContext.SaveChanges();
        }
    }
}
