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
    public class SubStatusMastersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubStatusMastersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SubStatusMasters
        public async Task<IActionResult> Index()
        {
            return View(await _context.SubStatus.ToListAsync());
        }

        // GET: SubStatusMasters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subStatusMaster = await _context.SubStatus
                .FirstOrDefaultAsync(m => m.ID == id);
            if (subStatusMaster == null)
            {
                return NotFound();
            }

            return View(subStatusMaster);
        }

        // GET: SubStatusMasters/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SubStatusMasters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,SubStatus,IsActive,StatusColor,ActionName,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate")] SubStatusMaster subStatusMaster)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subStatusMaster);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(subStatusMaster);
        }

        // GET: SubStatusMasters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subStatusMaster = await _context.SubStatus.FindAsync(id);
            if (subStatusMaster == null)
            {
                return NotFound();
            }
            return View(subStatusMaster);
        }

        // POST: SubStatusMasters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,SubStatus,IsActive,StatusColor,ActionName,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate")] SubStatusMaster subStatusMaster)
        {
            if (id != subStatusMaster.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subStatusMaster);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubStatusMasterExists(subStatusMaster.ID))
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
            return View(subStatusMaster);
        }

        // GET: SubStatusMasters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subStatusMaster = await _context.SubStatus
                .FirstOrDefaultAsync(m => m.ID == id);
            if (subStatusMaster == null)
            {
                return NotFound();
            }

            return View(subStatusMaster);
        }

        // POST: SubStatusMasters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subStatusMaster = await _context.SubStatus.FindAsync(id);
            if (subStatusMaster != null)
            {
                _context.SubStatus.Remove(subStatusMaster);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubStatusMasterExists(int id)
        {
            return _context.SubStatus.Any(e => e.ID == id);
        }
    }
}
