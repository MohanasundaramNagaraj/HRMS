using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SparkHRMS.Data;
using SparkHRMS.Data.Masters;

namespace SparkHRMS.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class WorkItemStatusController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkItemStatusController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WorkItemStatus
        public async Task<IActionResult> Index()
        {
            return View(await _context.WorkItemStatus.ToListAsync());
        }

        // GET: WorkItemStatus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var WorkItemStatus = await _context.WorkItemStatus
                .FirstOrDefaultAsync(m => m.ID == id);
            if (WorkItemStatus == null)
            {
                return NotFound();
            }

            return View(WorkItemStatus);
        }

        // GET: WorkItemStatus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WorkItemStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,WorkItemStatus,Descreption,IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate")] MST_WorkItemStatus WorkItemStatus)
        {
            if (ModelState.IsValid)
            {
                _context.Add(WorkItemStatus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(WorkItemStatus);
        }

        // GET: WorkItemStatus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var WorkItemStatus = await _context.WorkItemStatus.FindAsync(id);
            if (WorkItemStatus == null)
            {
                return NotFound();
            }
            return View(WorkItemStatus);
        }

        // POST: WorkItemStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,WorkItemStatus,Descreption,IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate")] MST_WorkItemStatus WorkItemStatus)
        {
            if (id != WorkItemStatus.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(WorkItemStatus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkItemStatusExists(WorkItemStatus.ID))
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
            return View(WorkItemStatus);
        }

        // GET: WorkItemStatus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var WorkItemStatus = await _context.WorkItemStatus
                .FirstOrDefaultAsync(m => m.ID == id);
            if (WorkItemStatus == null)
            {
                return NotFound();
            }

            return View(WorkItemStatus);
        }

        // POST: WorkItemStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var WorkItemStatus = await _context.WorkItemStatus.FindAsync(id);
            if (WorkItemStatus != null)
            {
                _context.WorkItemStatus.Remove(WorkItemStatus);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkItemStatusExists(int id)
        {
            return _context.WorkItemStatus.Any(e => e.ID == id);
        }
    }
}
