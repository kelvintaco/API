using Microsoft.AspNetCore.Mvc;
using APIFinal.Context;
using APIFinal.Models;

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

        [HttpPost]
        public void Post([FromBody] Transfer transfer)
        {
            _dataContext.Transfer.Add(transfer);
            _dataContext.SaveChanges();
        }
    }
}
