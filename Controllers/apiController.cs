using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebRestik.Models;

namespace WebRestik.Controllers
{
    [Route("api/[controller]")]
    public class apiController : Controller
    {
        private readonly RestaurantWebContext _context;

        public apiController(RestaurantWebContext context)
        {
            _context = context;
        }

        [HttpGet("getRestaurants")]
        async public Task<IActionResult> GetRestaurants()
        {
            var objects = await _context.Restaurants.ToListAsync(); 
            return Ok(objects);
        }
    }
}
