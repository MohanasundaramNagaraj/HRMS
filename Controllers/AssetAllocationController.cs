using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using SparkHRMS.Data;
using SparkHRMS.Data.Setting;
using SparkHRMS.Data.Masters;

public class AssetAllocationController : Controller
{
    private readonly ApplicationDbContext _context;

    public AssetAllocationController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: AssetAllocation
    public async Task<IActionResult> Index()
    {
        var allocations = await _context.AssetAllocations
                                        //.Include(a => a.Asset)
                                        .ToListAsync();
        return View(allocations);
    }

    // GET: AssetAllocation/Create
    public IActionResult Create()
    {
        ViewData["AssetId"] = new SelectList(_context.AssetMasters, "AssetId", "AssetName");
        ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Name");
        return View();
    }

    // POST: AssetAllocation/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AssetAllocation allocation)
    {
        if (ModelState.IsValid)
        {
            _context.Add(allocation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["AssetId"] = new SelectList(_context.AssetMasters, "AssetId", "AssetName", allocation.AssetId);
        ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Name", allocation.EmployeeId);
        return View(allocation);
    }

    // GET: AssetAllocation/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var allocation = await _context.AssetAllocations.FindAsync(id);
        if (allocation == null) return NotFound();

        ViewData["AssetId"] = new SelectList(_context.AssetMasters, "AssetId", "AssetName", allocation.AssetId);
        ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Name", allocation.EmployeeId);
        return View(allocation);
    }

    // POST: AssetAllocation/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AssetAllocation allocation)
    {
        if (id != allocation.AllocationId) return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(allocation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["AssetId"] = new SelectList(_context.AssetMasters, "AssetId", "AssetName", allocation.AssetId);
        ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Name", allocation.EmployeeId);
        return View(allocation);
    }

    // GET: AssetAllocation/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var allocation = await _context.AssetAllocations.FindAsync(id);
        if (allocation == null) return NotFound();
        return View(allocation);
    }

    // POST: AssetAllocation/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var allocation = await _context.AssetAllocations.FindAsync(id);
        if (allocation != null)
        {
            _context.AssetAllocations.Remove(allocation);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
