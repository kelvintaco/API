using Microsoft.AspNetCore.Mvc;
using APIFinal.Context;
using APIFinal.Models;

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
        
        [HttpPost]
        public void PostCustodian(Custodian custodian)
        {
            _dataContext.Custodians.Add(custodian);
            _dataContext.SaveChanges();
        }
    }
}
