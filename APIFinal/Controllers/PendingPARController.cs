using Microsoft.AspNetCore.Mvc;
using APIFinal.Context;
using APIFinal.Models;
using Microsoft.EntityFrameworkCore;

namespace APIFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PendingPARController : Controller
    {
        private readonly DataContext _dataContext;
        public PendingPARController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public List<PendingPAR> GetPendingPAR()
        {
            return _dataContext.PendingPAR.ToList();
        }
        [HttpPost]
        public void Post([FromBody] PendingPAR pendingpar)
        {
            _dataContext.PendingPAR.Add(pendingpar);
            _dataContext.SaveChanges();
        }
        [HttpDelete("byPendingParID/{parid}")]
        public void DeletebyPendingPARCode(string parid)
        {
            var toarchive = _dataContext.PendingPAR.FirstOrDefault(x => x.ParID == parid);
            _dataContext.PendingPAR.Remove(toarchive);
            _dataContext.SaveChanges();
        }
    }
}
