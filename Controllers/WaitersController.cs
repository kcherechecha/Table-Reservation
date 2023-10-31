using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebRestik.Models;

namespace WebRestik.Controllers
{
    public class WaitersController : Controller
    {
        private readonly RestaurantWebContext _context;

        public WaitersController(RestaurantWebContext context)
        {
            _context = context;
        }

        // GET: Waiters
        public async Task<IActionResult> Index()
        {
            var restaurantWebContext = _context.Waiters.Include(w => w.Restaurant);
            return View(await restaurantWebContext.ToListAsync());
        }

        // GET: Waiters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Waiters == null)
            {
                return NotFound();
            }

            var waiter = await _context.Waiters
                .Include(w => w.Restaurant)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (waiter == null)
            {
                return NotFound();
            }

            return View(waiter);
        }

        // GET: Waiters/Create
        public IActionResult Create()
        {
            ViewData["RestaurantId"] = new SelectList(_context.Restaurants, "Id", "Location");
            return View();
        }

        // POST: Waiters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,RestaurantId")] Waiter waiter)
        {
            if (ModelState.IsValid)
            {
                _context.Add(waiter);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RestaurantId"] = new SelectList(_context.Restaurants, "Id", "Location", waiter.RestaurantId);
            return View(waiter);
        }

        // GET: Waiters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Waiters == null)
            {
                return NotFound();
            }

            var waiter = await _context.Waiters.FindAsync(id);
            if (waiter == null)
            {
                return NotFound();
            }
            ViewData["RestaurantId"] = new SelectList(_context.Restaurants, "Id", "Location", waiter.RestaurantId);
            return View(waiter);
        }

        // POST: Waiters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,RestaurantId")] Waiter waiter)
        {
            if (id != waiter.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(waiter);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WaiterExists(waiter.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RestaurantId"] = new SelectList(_context.Restaurants, "Id", "Location", waiter.RestaurantId);
            return View(waiter);
        }

        // GET: Waiters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Waiters == null)
            {
                return NotFound();
            }

            var waiter = await _context.Waiters
                .Include(w => w.Restaurant)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (waiter == null)
            {
                return NotFound();
            }

            return View(waiter);
        }

        // POST: Waiters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Waiters == null)
            {
                return Problem("Entity set 'RestaurantWebContext.Waiters'  is null.");
            }
            var waiter = await _context.Waiters.FindAsync(id);
            if (waiter != null)
            {
                _context.Waiters.Remove(waiter);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WaiterExists(int id)
        {
          return _context.Waiters.Any(e => e.Id == id);
        }
    }
}
