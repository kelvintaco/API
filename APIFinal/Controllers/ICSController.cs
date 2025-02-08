using Microsoft.AspNetCore.Mvc;
using APIFinal.Context;
using APIFinal.Models;

namespace APIFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ICSController : Controller
    {
        private readonly DataContext _dataContext;
        public ICSController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public List<ICS> GetICS()
        {
            return _dataContext.ICS.ToList();
        }

        [HttpPost]
        public void PostICS(ICS ics)
        {
            _dataContext.ICS.Add(ics);
            _dataContext.SaveChanges();
        }
    }
}
