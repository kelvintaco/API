using Microsoft.AspNetCore.Mvc;
using APIFinal.Context;
using APIFinal.Models;

namespace APIFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemDisposalController : Controller
    {
        private readonly DataContext _dataContext;
        public ItemDisposalController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public List<ItemDisposal> GetItemDisposal()
        {
            return _dataContext.ItemDisposal.ToList();
        }

        [HttpPost]
        public void PostItemDisposal(ItemDisposal itemDisposal)
        {
            _dataContext.ItemDisposal.Add(itemDisposal);
            _dataContext.SaveChanges();
        }

    }
}
