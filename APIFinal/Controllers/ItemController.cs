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
        [HttpGet("byItemNames")]
        public List<string> GetItemNames()
        {
            return _dataContext.Items.Select(i => i.ItemName).ToList();
        }
        [HttpGet("getItemCode/{itemName}")]
        public ActionResult<int> GetItemCode(string itemName)
        {
            var item = _dataContext.Items.FirstOrDefault(i => i.ItemName == itemName);
            if (item == null)
            {
                return NotFound($"Item with name '{itemName}' not found.");
            }
            return Ok(item.ItemCode);
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
