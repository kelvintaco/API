using Microsoft.AspNetCore.Mvc;
using APIFinal.Context;
using APIFinal.Models;
using Microsoft.EntityFrameworkCore;

namespace APIFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PendingSurController : Controller
    {
        private readonly DataContext _dataContext;
        public PendingSurController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public List<PendingSur> GetPendingSur()
        {
            return _dataContext.PendingSur.ToList();
        }

        [HttpPost]
        public void Post([FromBody] PendingSur pendingsur)
        {
            _dataContext.PendingSur.Add(pendingsur);
            _dataContext.SaveChanges();
        }

        [HttpDelete("byPendingarchiveID/{archiveid}")]
        public void DeletebyPendingICSCode(int archiveid)
        {
            var toarchive = _dataContext.PendingSur.FirstOrDefault(x => x.archiveID == archiveid);
            _dataContext.PendingSur.Remove(toarchive);
            _dataContext.SaveChanges();
        }
    }
}
