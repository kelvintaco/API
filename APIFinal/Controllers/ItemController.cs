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
        [HttpGet("getItemsPriceByName/{itemName}")]
        public ActionResult<decimal> GetItemsPriceByName(string itemName)
        {
            var item = _dataContext.Items.FirstOrDefault(i => i.ItemName == itemName);
            if (item == null)
            {
                return NotFound($"Item with name '{itemName}' not found.");
            }
            return Ok(item.Price);
        }
        [HttpGet("getLifetimeByName/{itemName}")]
        public ActionResult<int> GetLifetimeByName(string itemName)
        {
            var item = _dataContext.Items.FirstOrDefault(i => i.ItemName == itemName);
            if (item == null)
            {
                return NotFound($"Item with name '{itemName}' not found.");
            }
            return Ok(item.LifeTime);
        }
        [HttpGet("getItemDescription/{itemName}")]
        public ActionResult<string> GetDescription(string itemName)
        {
            var item = _dataContext.Items.FirstOrDefault(i => i.ItemName == itemName);
            if (item == null)
            {
                return NotFound($"Item with name '{itemName}' not found.");
            }
            return Ok(item.ItemDeets);
        }
        [HttpGet("byItemNamesWithStock")]
        public List<Items> GetItemNamesWithStock()
        {
            return _dataContext.Items
                .Where(i => i.Stock > 0)
                .Select(i => new Items
                {
                    ItemName = i.ItemName,
                    Stock = i.Stock
                })
                .ToList();
        }

        [HttpGet("getStock/{itemName}")]
        public ActionResult<int> GetStock(string itemName)
        {
            var item = _dataContext.Items.FirstOrDefault(i => i.ItemName == itemName);
            if (item == null)
            {
                return NotFound($"Item with name '{itemName}' not found.");
            }
            return Ok(item.Stock);
        }

        [HttpPut("updateStock/{itemName}")]
        public IActionResult UpdateStock(string itemName, [FromBody] int quantity)
        {
            var item = _dataContext.Items.FirstOrDefault(i => i.ItemName == itemName);
            if (item == null)
            {
                return NotFound($"Item with name '{itemName}' not found.");
            }

            // Ensure stock doesn't go below 0
            if (item.Stock - quantity < 0)
            {
                return BadRequest($"Cannot reduce stock below 0. Current stock: {item.Stock}");
            }

            item.Stock -= quantity;
            _dataContext.SaveChanges();
            return Ok($"Stock updated for item '{itemName}'. New stock: {item.Stock}");
        }
        [HttpPost]
        public void PostItem(Items item)
        {
            _dataContext.Items.Add(item);
            _dataContext.SaveChanges();
        }

        [HttpPut("updatePlace/{itemName}")]
        public IActionResult UpdateItemPlace(string itemName, [FromBody] string place)
        {
            var item = _dataContext.Items.FirstOrDefault(i => i.ItemName == itemName);
            if (item == null)
            {
                return NotFound($"Item with name '{itemName}' not found.");
            }

            item.Place = place;
            _dataContext.SaveChanges();
            return Ok($"Place updated for item '{itemName}'");
        }
        [HttpPut("updateStockwhenSurrendered/{description}")]
        public async Task<IActionResult> UpdateStock(string description)
        {
            try
            {
                var item = _dataContext.Items
                    .FirstOrDefault(i => i.ItemDeets == description);

                if (item == null)
                {
                    return NotFound($"No item found with description: {description}");
                }

                // Increment the stock by 1
                item.Stock += 1;
                await _dataContext.SaveChangesAsync();
                return Ok($"Stock updated successfully. New stock: {item.Stock}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the stock");
            }
        }

        [HttpDelete("byItemCode/{itemCode}", Name = "DeletebyItemCode")]
        public void DeletebyItemCode(int itemcode)
        {
            var dispoitem = _dataContext.Items.FirstOrDefault(x => x.ItemCode == itemcode);
            _dataContext.Items.Remove(dispoitem);
            _dataContext.SaveChanges();
        }
        [HttpDelete("byItemName/{itemName}", Name = "DeletebyItemName")]
        public IActionResult DeletebyItemName(string itemName)
        {
            var item = _dataContext.Items.FirstOrDefault(i => i.ItemName == itemName);
            if (item == null)
            {
                return NotFound($"Item with name '{itemName}' not found.");
            }

            _dataContext.Items.Remove(item);
            _dataContext.SaveChanges();
            return Ok($"Item with name '{itemName}' has been deleted.");
        }
        [HttpDelete("byDescription/{description}")]
        public IActionResult DeleteByDescription(string description)
        {
            try
            {
                // Find the item by description
                var item = _dataContext.Items
                    .FirstOrDefault(i => i.ItemDeets == description);

                if (item == null)
                {
                    return NotFound($"No item found with description: {description}");
                }

                // Remove the item
                _dataContext.Items.Remove(item);
                _dataContext.SaveChangesAsync();

                return Ok($"Item with description {description} has been deleted");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while deleting the item");
            }
        }
    }
}
