using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using SparkHRMS.Data;
using SparkHRMS.Models;
using SparkHRMS.Data.Masters;
using Microsoft.AspNetCore.Authorization;

[Authorize(Roles = "SuperAdmin,Admin")]
public class AssetMasterController : Controller
{
    private readonly ApplicationDbContext _context;

    public AssetMasterController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: AssetMaster
    public async Task<IActionResult> Index()
    {
        var assets = await _context.AssetMasters.ToListAsync();
        return View(assets);
    }

    // GET: AssetMaster/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: AssetMaster/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AssetMaster asset)
    {
        if (ModelState.IsValid)
        {
            _context.Add(asset);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(asset);
    }

    // GET: AssetMaster/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var asset = await _context.AssetMasters.FindAsync(id);
        if (asset == null) return NotFound();
        return View(asset);
    }

    // GET: AssetMaster/View/5
    public async Task<IActionResult> View(int? id)
    {
        if (id == null) return NotFound();
        var asset = await _context.AssetMasters.FindAsync(id);
        if (asset == null) return NotFound();
        return View(asset);
    }

    // POST: AssetMaster/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AssetMaster asset)
    {
        if (id != asset.AssetId) return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(asset);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(asset);
    }

    // GET: AssetMaster/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var asset = await _context.AssetMasters.FindAsync(id);
        if (asset == null) return NotFound();
        return View(asset);
    }

    // POST: AssetMaster/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var asset = await _context.AssetMasters.FindAsync(id);
        if (asset != null)
        {
            _context.AssetMasters.Remove(asset);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
