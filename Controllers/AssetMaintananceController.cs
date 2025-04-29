using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using SparkHRMS.Data;
using SparkHRMS.Data.Setting;
using SparkHRMS.Data.Masters;

public class AssetMaintenanceController : Controller
{
    private readonly ApplicationDbContext _context;

    public AssetMaintenanceController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: AssetMaintenance
    public async Task<IActionResult> Index()
    {
        var maintenanceRecords = await _context.AssetMaintenances
                                               //.Include(m => m.AssetId)
                                               .ToListAsync();
        return View(maintenanceRecords);
    }

    // GET: AssetMaintenance/Create
    public IActionResult Create()
    {
        ViewData["AssetId"] = new SelectList(_context.AssetMasters, "AssetId", "AssetName");
        return View();
    }

    // POST: AssetMaintenance/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AssetMaintenance maintenance)
    {
        if (ModelState.IsValid)
        {
            _context.Add(maintenance);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["AssetId"] = new SelectList(_context.AssetMasters, "AssetId", "AssetName", maintenance.AssetId);
        return View(maintenance);
    }

    // GET: AssetMaintenance/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var maintenance = await _context.AssetMaintenances.FindAsync(id);
        if (maintenance == null) return NotFound();

        ViewData["AssetId"] = new SelectList(_context.AssetMasters, "AssetId", "AssetName", maintenance.AssetId);
        return View(maintenance);
    }

    // POST: AssetMaintenance/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AssetMaintenance maintenance)
    {
        if (id != maintenance.MaintenanceId) return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(maintenance);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["AssetId"] = new SelectList(_context.AssetMasters, "AssetId", "AssetName", maintenance.AssetId);
        return View(maintenance);
    }

    // GET: AssetMaintenance/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var maintenance = await _context.AssetMaintenances.FindAsync(id);
        if (maintenance == null) return NotFound();
        return View(maintenance);
    }

    // POST: AssetMaintenance/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var maintenance = await _context.AssetMaintenances.FindAsync(id);
        if (maintenance != null)
        {
            _context.AssetMaintenances.Remove(maintenance);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
