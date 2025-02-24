using Microsoft.AspNetCore.Mvc;
using APIFinal.Context;
using APIFinal.Models;

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

        [HttpPost]
        public void Post([FromBody] PAR par)
        {
            _dataContext.PAR.Add(par);
            _dataContext.SaveChanges();
        }

        [HttpDelete("byParID/{parCode}", Name = "DeletebyParID")]
        public void DeletebyPARCode(string parid)
        {
            var toarchive = _dataContext.PAR.FirstOrDefault(x => x.ParID == parid);
            _dataContext.PAR.Remove(toarchive);
            _dataContext.SaveChanges();
        }
    }
}
