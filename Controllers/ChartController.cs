using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebRestik.Models;

namespace WebRestik.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartController : Controller
    {
        private readonly RestaurantWebContext _context;

        public ChartController(RestaurantWebContext context)
        {
            _context = context;
        }

        [HttpGet("JsonDataRestaurant")]
        public JsonResult JsonDataRestaurant() 
        {
            var restaurants = _context.Restaurants.Include(r => r.Tables).ToList();
            List<object> tables = new List<object>();
            tables.Add(new[] { "Ресторан", "Кількість столиків" });
            foreach(var r in restaurants)
            {
                tables.Add(new object[] { r.Name, r.Tables.Count() });
            }

            return new JsonResult(tables);
        }

    }
}
