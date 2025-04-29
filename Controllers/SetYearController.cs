using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SparkHRMS.Data;
using SparkHRMS.Data.Setting;

namespace SparkHRMS.Controllers
{
    public class SetYearController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SetYearController(ApplicationDbContext context)
        {
            _context = context;
        }

        // List All Years
        public async Task<IActionResult> Index()
        {
            var years = await _context.Year.ToListAsync();
            return View(years);
        }

        // Create GET
        public IActionResult Create()
        {
            return View(new SetYear());
        }

        // Create POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SetYear setYear)
        {
            if (ModelState.IsValid)
            {
                _context.Year.Add(setYear);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(setYear);
        }

        // Edit GET
        public async Task<IActionResult> Edit(int id)
        {
            var setYear = await _context.Year.FindAsync(id);
            if (setYear == null)
                return NotFound();

            return View(setYear);
        }

        // Edit POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SetYear setYear)
        {
            if (id != setYear.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(setYear);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(setYear);
        }

        // Delete GET
        public async Task<IActionResult> Delete(int id)
        {
            var setYear = await _context.Year.FindAsync(id);
            if (setYear == null)
                return NotFound();

            return View(setYear);
        }

        // Delete POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var setYear = await _context.Year.FindAsync(id);
            if (setYear != null)
            {
                _context.Year.Remove(setYear);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
