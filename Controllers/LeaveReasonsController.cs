using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SparkHRMS.Data;
using SparkHRMS.Data.Masters;

namespace SparkHRMS.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class LeaveReasonsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeaveReasonsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Index
        public async Task<IActionResult> Index()
        {
            return View(await _context.LeaveReasons.ToListAsync());
        }

        // Create - GET
        public IActionResult Create()
        {
            return View();
        }

        // Create - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeaveReasons reason)
        {
            if (ModelState.IsValid)
            {
                reason.CreatedDate = DateTime.Now;
                reason.CreatedBy = 1; // Replace with actual user id
                _context.Add(reason);
                await _context.SaveChangesAsync();
              
                return RedirectToAction("Index", "LeaveReasons");
            }
            return View(reason);
        }

        // Edit - GET
        public async Task<IActionResult> Edit(int id)
        {
            var reason = await _context.LeaveReasons.FindAsync(id);
            if (reason == null)
                return NotFound();

            return View(reason);
        }

        // Edit - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LeaveReasons reason)
        {
            if (id != reason.LeaveReasonId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    reason.LastUpdatedDate = DateTime.Now;
                    reason.UpdatedBy = 1; // Replace with actual user id
                    _context.Update(reason);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.LeaveReasons.Any(e => e.LeaveReasonId == id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction("Index", "LeaveReasons");
            }
            return View(reason);
        }

        // Delete - GET
        public async Task<IActionResult> Delete(int id)
        {
            var reason = await _context.LeaveReasons.FindAsync(id);
            if (reason == null)
                return NotFound();

            return View(reason);
        }

        // Delete - POST
     
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reason = await _context.LeaveReasons.FindAsync(id);
            _context.LeaveReasons.Remove(reason);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "LeaveReasons");
        }
    }

}
