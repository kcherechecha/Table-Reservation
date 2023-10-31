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
    public class ReservationsController : Controller
    {
        private readonly RestaurantWebContext _context;

        public ReservationsController(RestaurantWebContext context)
        {
            _context = context;
        }

        // GET: Reservations
        public async Task<IActionResult> Index()
        {
            var restaurantWebContext = _context.Reservations.Include(r => r.Table).Include(r => r.Waiter);
            return View(await restaurantWebContext.ToListAsync());
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Reservations == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Table)
                .Include(r => r.Waiter)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        public IActionResult Create(int tableId)
        {
            var table = _context.Tables.FirstOrDefault(t => t.Id == tableId);
            var restaurantId = _context.Restaurants.First(r => r.Id == table.RestaurantId).Id;

            ViewData["WaiterId"] = new SelectList(_context.Waiters.Where(r => r.RestaurantId == restaurantId), "Id", "Name");

            ViewBag.TableId = tableId;

            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BookingTime,TableId,ClientName,WaiterId")] Reservation reservation)
        {
            ViewBag.TableId = reservation.TableId;
            if (ModelState.IsValid)
            {
                var dateIsBusy = _context.Reservations.Where(r => r.BookingTime.AddHours(3) >= reservation.BookingTime && r.BookingTime <= reservation.BookingTime).Any();

                var table = _context.Tables.FirstOrDefault(t => t.Id == reservation.TableId);
                var restaurantId = _context.Restaurants.First(r => r.Id == table.RestaurantId).Id;

                if (!dateIsBusy)
                {
                    _context.Add(reservation);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Tables", new {restaurantId = restaurantId} );
                }
                else
                {
                    ModelState.AddModelError("BookingTime", "Обраний час недоступний, так як столик вже заброньований.");
                    ViewData["WaiterId"] = new SelectList(_context.Waiters.Where(r => r.RestaurantId == restaurantId), "Id", "Name");
                    return View(reservation);
                }
            }
            ViewData["TableId"] = new SelectList(_context.Tables, "Id", "Number", reservation.TableId);
            ViewData["WaiterId"] = new SelectList(_context.Waiters, "Id", "Name", reservation.WaiterId);
            return View(reservation);
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Reservations == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            ViewData["TableId"] = new SelectList(_context.Tables, "Id", "Number", reservation.TableId);
            ViewData["WaiterId"] = new SelectList(_context.Waiters, "Id", "Name", reservation.WaiterId);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BookingTime,TableId,ClientName,WaiterId")] Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.Id))
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
            ViewData["TableId"] = new SelectList(_context.Tables, "Id", "Number", reservation.TableId);
            ViewData["WaiterId"] = new SelectList(_context.Waiters, "Id", "Name", reservation.WaiterId);
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Reservations == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Table)
                .Include(r => r.Waiter)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Reservations == null)
            {
                return Problem("Entity set 'RestaurantWebContext.Reservations'  is null.");
            }
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
          return _context.Reservations.Any(e => e.Id == id);
        }
    }
}
