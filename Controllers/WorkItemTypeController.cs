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
    public class WorkItemTypeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkItemTypeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WorkItemType
        public async Task<IActionResult> Index()
        {
            return View(await _context.WorkItemType.ToListAsync());
        }

        // GET: WorkItemType/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var WorkItemType = await _context.WorkItemType
                .FirstOrDefaultAsync(m => m.ID == id);
            if (WorkItemType == null)
            {
                return NotFound();
            }

            return View(WorkItemType);
        }

        // GET: WorkItemType/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WorkItemType/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,WorkItemTypeName,Description,IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate")] MST_WorkItemType WorkItemType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(WorkItemType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(WorkItemType);
        }

        // GET: WorkItemType/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var WorkItemType = await _context.WorkItemType.FindAsync(id);
            if (WorkItemType == null)
            {
                return NotFound();
            }
            return View(WorkItemType);
        }

        // POST: WorkItemType/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,WorkItemTypeName,Description,IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate")] MST_WorkItemType WorkItemType)
        {
            if (id != WorkItemType.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(WorkItemType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkItemTypeExists(WorkItemType.ID))
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
            return View(WorkItemType);
        }

        // GET: WorkItemType/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var WorkItemType = await _context.WorkItemType
                .FirstOrDefaultAsync(m => m.ID == id);
            if (WorkItemType == null)
            {
                return NotFound();
            }

            return View(WorkItemType);
        }

        // POST: WorkItemType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var WorkItemType = await _context.WorkItemType.FindAsync(id);
            if (WorkItemType != null)
            {
                _context.WorkItemType.Remove(WorkItemType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkItemTypeExists(int id)
        {
            return _context.WorkItemType.Any(e => e.ID == id);
        }
    }
}
