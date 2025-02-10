using Microsoft.AspNetCore.Mvc;
using APIFinal.Context;
using APIFinal.Models;

namespace APIFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArchivedTransactionsController : Controller
    {
        private readonly DataContext _dataContext;
        public ArchivedTransactionsController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public List<ArchivedTransactions> GetArchivedTransactions()
        {
            return _dataContext.ArchievedTransactions.ToList();
        }
        [HttpPost]
        public void PostArchivedTransactions(ArchivedTransactions archivedTransactions)
        {
            _dataContext.ArchievedTransactions.Add(archivedTransactions);
            _dataContext.SaveChanges();
        }
    }
}
