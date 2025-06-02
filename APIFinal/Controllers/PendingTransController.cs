using Microsoft.AspNetCore.Mvc;
using APIFinal.Context;
using APIFinal.Models;
using Microsoft.EntityFrameworkCore;

namespace APIFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PendingTransController : Controller
    {
        private readonly DataContext _dataContext;
        public PendingTransController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public List<PendingTrans> GetPendingSur()
        {
            return _dataContext.PendingTrans.ToList();
        }

        [HttpPost]
        public void Post([FromBody] PendingTrans pendingtrans)
        {
            _dataContext.PendingTrans.Add(pendingtrans);
            _dataContext.SaveChanges();
        }

        [HttpDelete("byPendingptrid/{ptrid}")]
        public void DeletebyPendingICSCode(int ptrid)
        {
            var toarchive = _dataContext.PendingTrans.FirstOrDefault(x => x.PtrId == ptrid);
            _dataContext.PendingTrans.Remove(toarchive);
            _dataContext.SaveChanges();
        }
    }
}
