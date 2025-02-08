using Microsoft.AspNetCore.Mvc;
using APIFinal.Context;
using APIFinal.Models;

namespace APIFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : Controller
    {
        private readonly DataContext _dataContext;
        public ItemController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public List<Items> GetItem()
        {
            return _dataContext.Items.ToList();
        }

        [HttpPost]
        public void PostItem(Items item)
        {
            _dataContext.Items.Add(item);
            _dataContext.SaveChanges();
        }

        [HttpDelete("byItemCode/{itemCode}", Name = "DeletebyItemCode")]
        public void DeletebyItemCode(int itemcode)
        {
            var dispoitem = _dataContext.Items.FirstOrDefault(x => x.ItemCode == itemcode);
            _dataContext.Items.Remove(dispoitem);
            _dataContext.SaveChanges();
        }
    }
}
