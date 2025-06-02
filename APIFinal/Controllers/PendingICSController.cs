using Microsoft.AspNetCore.Mvc;
using APIFinal.Context;
using APIFinal.Models;
using Microsoft.EntityFrameworkCore;

namespace APIFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PendingICSController : Controller
    {
        private readonly DataContext _dataContext;
        public PendingICSController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public List<PendingICS> GetPendingICS()
        {
            return _dataContext.PendingICS.ToList();
        }
        [HttpPost]
        public void Post([FromBody] PendingICS pendingics)
        {
            _dataContext.PendingICS.Add(pendingics);
            _dataContext.SaveChanges();
        }
        [HttpDelete("byPendingICScode/{icscode}")]
        public void DeletebyPendingICSCode(int icscode)
        {
            var toarchive = _dataContext.PendingICS.FirstOrDefault(x => x.ICSID == icscode);
            _dataContext.PendingICS.Remove(toarchive);
            _dataContext.SaveChanges();
        }
    }
}
